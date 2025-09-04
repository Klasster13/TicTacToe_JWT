using TicTacToe.Common.Enums;
using TicTacToe.Domain.Models;
using TicTacToe.Web.Models.Requests;
using TicTacToe.Web.Models.Responses;

namespace TicTacToe.Web.Mappers;

public static class SessionWebMapper
{
    public static SessionResponse ToWebModel(this Session session)
    {
        ArgumentNullException.ThrowIfNull(session, nameof(session));

        return new SessionResponse
        {
            Id = session.Id,
            Mode = session.Mode,
            Difficulty = session.Difficulty,
            Field = session.Field.ToWebModel(),
            State = session.State,
            Player1Id = session.Player1Id,
            Player2Id = session.Player2Id,
            WinningCells = session.WinningCells,
            CreatorId = session.CreatorId
        };
    }

    public static IEnumerable<SessionResponse> ToWebModel(this IEnumerable<Session> sessions) =>
        sessions.Select(s => s.ToWebModel());
}

public static class FieldWebMapper
{

    public static FieldWeb ToWebModel(this Field field)
    {
        return new FieldWeb
        {
            Board = field.Board,
        };
    }


    public static Field ToDomainModel(this FieldWeb? request)
    {
        return request?.Board is null ? new Field() : new Field { Board = request.Board };
    }


    public static Field ToDomainModel(this MoveRequest request) => new() { Board = request.Field };
}


public static class CreateGameWebMapper
{
    public static Session ToDomainModel(this CreateSessionRequest request, Guid userId) => new()
    {
        Id = Guid.NewGuid(),
        Mode = request.Mode,
        Difficulty = request.Difficulty,
        Field = new Field(),
        State = request.Mode == Mode.TwoPlayers ? State.WaitingForPlayers : State.Player1Turn,
        Player1Id = request.StartSide == Player.Player1 ? userId : null,
        Player2Id = request.StartSide == Player.Player2 ? userId : null,
        CreatorId = userId
    };
}


public static class UserWebMapper
{
    public static User ToDomainModel(this SignUpRequest request) => new()
    {
        Id = Guid.NewGuid(),
        Login = request.Login,
        Password = request.Password
    };

    public static UserResponse ToWebModel(this User user) => new()
    {
        Id = user.Id,
        Login = user.Login
    };

    public static User ToDomainModel(this UpdateUserRequest request, Guid userId) => new()
    {
        Id = userId,
        Login = request.Login ?? string.Empty,
        Password = request.Password ?? string.Empty,
    };

    public static User ToDomainModel(this JwtRequest request) => new()
    {
        Login = request.Login,
        Password = request.Password
    };
}


public static class JwtTokenMapper
{
    public static JwtResponse ToWebModel(this JwtToken token) => new()
    {
        AccessToken = token.AccessToken,
        RefreshToken = token.RefreshToken
    };
}


public static class WinRatioMapper
{
    public static WinRatioResponse ToWebModel(this WinRatio winRatio) => new()
    {
        Id = winRatio.Id,
        Login = winRatio.Login,
        WinRatio = winRatio.Ratio
    };
}