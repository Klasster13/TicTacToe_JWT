using TicTacToe.Common.Enums;
using TicTacToe.Common.Models;
using TicTacToe.DataSource.Service;
using TicTacToe.Domain.Models;

namespace TicTacToe.Domain.Services.GameService.Impl;

public class SessionService(IDataService dataService) : ISessionService
{
    private const int MaxScore = 10;

    private readonly IDataService _dataService = dataService;


    public async Task<Session?> GetSession(Guid sessionId) => await _dataService.GetSession(sessionId);
    public async Task DeleteSession(Guid sessionId) => await _dataService.DeleteSession(sessionId);
    public async Task<IEnumerable<Session>> GetUserSessions(Guid userId) => await _dataService.GetUserSessions(userId);
    public async Task<IEnumerable<Session>> GetAvailableSessions(Guid userId) => await _dataService.GetAvailableSessions(userId);


    public async Task<Session> CreateSession(Session session)
    {
        if (session?.Field?.Board is null)
            throw new ArgumentNullException(nameof(session), "Session is corrupted.");

        await _dataService.AddSession(session);
        return session;
    }

    public async Task<Session?> ValidateMove(Guid userId, Guid sessionId, Field newField)
    {
        var savedSession = await _dataService.GetSession(sessionId)
            ?? throw new KeyNotFoundException($"Session {sessionId} not found");

        if (savedSession.State != State.Player1Turn && savedSession.State != State.Player2Turn)
        {
            throw new InvalidOperationException("Can't make move. Game is over");
        }

        bool isSameUser = savedSession.Mode == Mode.TwoPlayers
            ? savedSession.State switch
            {
                State.Player1Turn => savedSession.Player1Id == userId,
                State.Player2Turn => savedSession.Player2Id == userId,
                State.WaitingForPlayers => throw new InvalidOperationException("Join second player first"),
                _ => throw new InvalidOperationException($"Unexpected game state: {savedSession.State}")
            }
            : savedSession.CreatorId == userId
                && (savedSession.Player1Id == userId || savedSession.Player2Id == userId);

        if (!isSameUser)
        {
            throw new InvalidOperationException("User is not allowed to modify this session");
        }

        var oldField = savedSession.Field;
        int differences = 0;

        for (int y = 0; y < Field.Size; y++)
        {
            for (int x = 0; x < Field.Size; x++)
            {
                var oldCell = oldField[y, x];
                var newCell = newField[y, x];

                if (oldCell != Cell.None && oldCell != newCell)
                {
                    throw new ArgumentException($"Cell value mismatch (y={y},x={x})");
                }

                if (oldCell != newCell)
                {
                    if (++differences > 1)
                        throw new ArgumentException($"Too many differences");
                }
            }
        }

        savedSession.Field = newField;
        return savedSession;
    }


    public async Task<Session> MakeMove(Session session)
    {
        if (session.State == State.WaitingForPlayers) return session;

        if (session.Mode == Mode.OnePlayer)
        {
            await HandleOnePlayer(session);
        }
        else if (session.Mode == Mode.TwoPlayers)
        {
            await HandleTwoPlayers(session);
        }
        return session;
    }


    private async Task HandleTwoPlayers(Session session)
    {
        var state = IsGameOver(session);
        if (state != State.Player1Turn && state != State.Player2Turn)
        {
            session.WinningCells = GetWinningCombination(session);
            session.State = state;
        }
        else
        {
            session.State = session.State == State.Player1Turn
                ? State.Player2Turn
                : State.Player1Turn;
        }
        await _dataService.UpdateSession(session);
    }


    private async Task HandleOnePlayer(Session session)
    {
        // Check field state after player's move
        var state = IsGameOver(session);
        if (state != State.Player1Turn && state != State.Player2Turn)
        {
            session.State = state;
            session.WinningCells = GetWinningCombination(session);
        }
        else
        {
            if (!IsEmptyField(session.Field))
            {
                // Change state for verter move
                session.State = session.State == State.Player1Turn
                    ? State.Player2Turn
                    : State.Player1Turn;
            }

            if (session.Difficulty == Difficulty.Easy)
            {
                MakeRandomAiMove(session);
            }
            else if (session.Difficulty == Difficulty.Medium)
            {
                MakeMixedAiMove(session);
            }
            else
            {
                MakeMinMaxAiMove(session);
            }

            // Check field state after verter move
            state = IsGameOver(session);
            if (state != State.Player1Turn && state != State.Player2Turn)
            {
                session.State = state;
                session.WinningCells = GetWinningCombination(session);
            }
            else
            {
                // Change state back to player
                session.State = session.State == State.Player1Turn
                    ? State.Player2Turn
                    : State.Player1Turn;
            }
        }
        await _dataService.UpdateSession(session);
    }


    private static bool IsEmptyField(Field field)
    {
        if (field.Board.All(row => row.All(cell => cell == Cell.None)))
        {
            return true;
        }

        return false;
    }


    private static State IsGameOver(Session session)
    {
        if (session?.Field?.Board is null)
            throw new ArgumentNullException(nameof(session), "Session is corrupted.");

        var field = session.Field;

        bool diag1WinX = true;
        bool diag1WinO = true;
        bool diag2WinX = true;
        bool diag2WinO = true;
        bool hasFreeCells = false;

        for (int y = 0; y < Field.Size; y++)
        {
            bool rowWinX = true, rowWinO = true;
            bool colWinX = true, colWinO = true;

            for (int x = 0; x < Field.Size; x++)
            {
                // Check row
                if (field[y, x] != Cell.X) rowWinX = false;
                if (field[y, x] != Cell.O) rowWinO = false;

                // Check col
                if (field[x, y] != Cell.X) colWinX = false;
                if (field[x, y] != Cell.O) colWinO = false;

                // Check free cells
                if (field[y, x] == Cell.None) hasFreeCells = true;
            }

            if (rowWinX || colWinX) return State.Player1Winner;
            if (rowWinO || colWinO) return State.Player2Winner;

            // Check diagonals
            if (field[y, y] != Cell.X) diag1WinX = false;
            if (field[y, y] != Cell.O) diag1WinO = false;
            if (field[y, Field.Size - 1 - y] != Cell.X) diag2WinX = false;
            if (field[y, Field.Size - 1 - y] != Cell.O) diag2WinO = false;
        }

        if (diag1WinX || diag2WinX) return State.Player1Winner;
        if (diag1WinO || diag2WinO) return State.Player2Winner;

        return hasFreeCells
            ? session.State
            : State.Draw;
    }


    private static void MakeRandomAiMove(Session session)
    {
        Cell verterSymbol = session.State switch
        {
            State.Player1Turn => Cell.X,
            State.Player2Turn => Cell.O,
            _ => throw new InvalidOperationException("AI move is not allowed in current state")
        };

        var availableMoves = GetAvailableMoves(session);
        if (availableMoves.Count == 0) return;

        Random random = new();

        var randomMove = availableMoves.ElementAt(random.Next(availableMoves.Count));
        session.Field[randomMove.Y, randomMove.X] = verterSymbol;
    }


    private static void MakeMixedAiMove(Session session)
    {
        Random random = new();

        if (random.Next(100) < 30)
        {
            MakeRandomAiMove(session);
        }
        else
        {
            MakeMinMaxAiMove(session);
        }
    }

    private static void MakeMinMaxAiMove(Session session)
    {
        Cell verterSymbol = session.State switch
        {
            State.Player1Turn => Cell.X,
            State.Player2Turn => Cell.O,
            _ => throw new InvalidOperationException("AI move is not allowed in current state")
        };

        Point? bestMove;

        (bestMove, _) = Minimax(session, isMax: true, verterSymbol, 0);

        if (bestMove is not null)
        {
            session.Field[bestMove.Y, bestMove.X] = verterSymbol;
        }
    }


    private static (Point? move, int score) Minimax(Session session, bool isMax, Cell playerSymbol, int depth)
    {
        var gameState = IsGameOver(session);

        if (gameState != State.Player1Turn && gameState != State.Player2Turn)
        {
            return (null, GetScore(gameState, playerSymbol, depth));
        }

        depth++;
        var field = session.Field;
        var availableMoves = GetAvailableMoves(session);

        int score;
        List<int> scores = [];
        HashSet<Point> moves = [];

        foreach (var move in availableMoves)
        {
            if (isMax)
            {
                field[move.Y, move.X] = playerSymbol;
                (_, score) = Minimax(session, isMax: false, playerSymbol, depth);
                field[move.Y, move.X] = Cell.None;
            }
            else
            {
                field[move.Y, move.X] = playerSymbol == Cell.X
                    ? Cell.O
                    : Cell.X;
                (_, score) = Minimax(session, isMax: true, playerSymbol, depth);
                field[move.Y, move.X] = Cell.None;
            }
            scores.Add(score);
            moves.Add(move);
        }

        int finalScore = isMax ? scores.Max() : scores.Min();
        var random = new Random();

        var bestMove = moves
            .Select((move, i) => (move, score: scores[i]))
            .Where(x => x.score == finalScore)
            .OrderBy(_ => random.Next())
            .First().move;

        return (bestMove, finalScore);
    }


    private static int GetScore(State gameState, Cell playerSymbol, int depth)
    {
        return gameState switch
        {
            State.Player1Winner => playerSymbol == Cell.X ? MaxScore - depth : depth - MaxScore,
            State.Player2Winner => playerSymbol == Cell.O ? MaxScore - depth : depth - MaxScore,
            _ => 0
        };
    }


    private static HashSet<Point> GetAvailableMoves(Session session)
    {
        var field = session.Field;

        var moves = new HashSet<Point>();
        for (int y = 0; y < Field.Size; y++)
        {
            for (int x = 0; x < Field.Size; x++)
            {
                if (field[y, x] == Cell.None)
                    moves.Add(new Point(y, x));
            }
        }
        return moves;
    }


    private static List<Point>? GetWinningCombination(Session session)
    {
        var field = session.Field;

        for (int x = 0; x < 3; x++)
        {
            if (field[0, x] != 0 && field[0, x] == field[1, x] && field[1, x] == field[2, x])
            {
                return [new Point(0, x),
                        new Point(1, x),
                        new Point(2, x)];
            }
        }

        for (int y = 0; y < 3; y++)
        {
            if (field[y, 0] != 0 && field[y, 0] == field[y, 1] && field[y, 1] == field[y, 2])
            {
                return [new Point(y, 0),
                        new Point(y, 1),
                        new Point(y, 2)];
            }
        }

        if (field[0, 0] != 0 && field[0, 0] == field[1, 1] && field[1, 1] == field[2, 2])
        {
            return [new Point(0, 0),
                    new Point(1, 1),
                    new Point(2, 2)];
        }

        if (field[2, 0] != 0 && field[2, 0] == field[1, 1] && field[1, 1] == field[0, 2])
        {
            return [new Point(2, 0),
                    new Point(1, 1),
                    new Point(0, 2)];
        }

        return null;
    }


    public async Task<Session?> AddPlayerToSession(Guid sessionId, Guid userId)
    {
        var session = await _dataService.GetSession(sessionId)
            ?? throw new KeyNotFoundException($"Session {sessionId} not found");

        if (session.Mode != Mode.TwoPlayers)
        {
            throw new InvalidOperationException($"Invalid session mode: {session.Mode}");
        }

        if (session.Player1Id == userId || session.Player2Id == userId)
        {
            throw new InvalidOperationException($"User {userId} already in session");
        }

        if (session.Player1Id is not null && session.Player2Id is not null)
        {
            throw new InvalidOperationException($"Session {sessionId} is full");
        }

        if (session.Player1Id is null)
        {
            session.Player1Id = userId;
        }
        else
        {
            session.Player2Id = userId;
        }

        session.State = State.Player1Turn;
        await _dataService.UpdateSession(session);
        return session;
    }


    public async Task<Session> UpdateSessionMode(Guid sessionId, Guid userId, Mode newMode)
    {
        var session = await _dataService.GetSession(sessionId)
            ?? throw new KeyNotFoundException($"Session {sessionId} not found");

        if (session.CreatorId != userId)
        {
            throw new InvalidOperationException("User is not allowed to modify session");
        }

        if (session.Player1Id != userId && session.Player2Id != userId)
        {
            throw new InvalidOperationException("User is not presented in session");
        }

        if (session.State == State.Draw
            || session.State == State.Player1Winner
            || session.State == State.Player2Winner)
        {
            throw new InvalidOperationException("Game is over");
        }

        if (session.Mode == newMode)
        {
            return session;
        }

        var isCreatorPlayer1 = session.Player1Id == session.CreatorId;

        if (newMode == Mode.OnePlayer)
        {
            session.Player1Id = isCreatorPlayer1 ? session.Player1Id : null;
            session.Player2Id = isCreatorPlayer1 ? null : session.Player2Id;

            if (session.State == State.WaitingForPlayers)
            {
                session.State = State.Player1Turn;
            }
        }
        else
        {
            session.State = State.WaitingForPlayers;
        }

        session.Mode = newMode;
        await _dataService.UpdateSession(session);
        return session;
    }

    public async Task<Session> ResetSession(Guid sessionId, Guid userId)
    {
        var session = await _dataService.GetSession(sessionId)
            ?? throw new KeyNotFoundException($"Session {sessionId} not found");


        if (session.Mode == Mode.OnePlayer && session.CreatorId != userId)
        {
            throw new InvalidOperationException("User is not allowed to modify session");
        }
        else if (session.Mode == Mode.TwoPlayers
            && session.Player1Id != userId
            && session.Player2Id != userId)
        {
            throw new InvalidOperationException("User is not allowed to modify session");
        }

        session.Field = new();
        session.State = State.Player1Turn;
        session.WinningCells = default;

        await _dataService.UpdateSession(session);

        return session;
    }
}