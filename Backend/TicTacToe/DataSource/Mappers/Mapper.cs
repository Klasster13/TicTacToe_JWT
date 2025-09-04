using System.Text;
using TicTacToe.Common.Enums;
using TicTacToe.Common.Models;
using TicTacToe.DataSource.Models;
using TicTacToe.Domain.Models;

namespace TicTacToe.DataSource.Mappers;

public static class SessionDataMapper
{
    public static SessionData ToDataModel(this Session session) => new()
    {
        Id = session.Id,
        Mode = session.Mode,
        Difficulty = session.Difficulty,
        Field = session.Field.ToDataModel(),
        State = session.State,
        Player1Id = session.Player1Id,
        Player2Id = session.Player2Id,
        WinningCells = session.WinningCells?.CellsToDataModel(),
        CreatorId = session.CreatorId,
        CreatedAt = session.CreatedAt,
        UpdatedAt = session.UpdatedAt
    };

    public static Session ToDomainModel(this SessionData data) => new()
    {
        Id = data.Id,
        Mode = data.Mode,
        Difficulty = data.Difficulty,
        Field = data.Field.ToDomainModel(),
        State = data.State,
        Player1Id = data.Player1Id,
        Player2Id = data.Player2Id,
        WinningCells = data.WinningCells?.CellsToDomainModel(),
        CreatorId = data.CreatorId,
        CreatedAt = data.CreatedAt,
        UpdatedAt = data.UpdatedAt
    };
}

public static class FieldDataMapper
{
    public static string ToDataModel(this Field field)
    {
        StringBuilder sb = new();

        for (int i = 0; i < Field.Size; i++)
        {
            for (int j = 0; j < Field.Size; j++)
            {
                char c = field[i, j] switch
                {
                    Cell.X => 'X',
                    Cell.O => 'O',
                    _ => '-'
                };

                sb.Append(c);
            }
        }
        return sb.ToString();
    }


    public static Field ToDomainModel(this string data)
    {
        var field = new Field();
        for (int i = 0; i < Field.Size; i++)
        {
            for (int j = 0; j < Field.Size; j++)
            {
                Cell cell = data[i * Field.Size + j] switch
                {
                    'X' => Cell.X,
                    'O' => Cell.O,
                    _ => Cell.None
                };
                field[i, j] = cell;
            }
        }
        return field;
    }
}


public static class WinningCellsDataMapper
{
    public static string CellsToDataModel(this List<Point> list)
    {
        StringBuilder sb = new();

        foreach (Point p in list)
        {
            sb.Append(p.Y * 3 + p.X);
        }

        return sb.ToString();
    }

    public static List<Point> CellsToDomainModel(this string data)
    {
        var list = new List<Point>();

        foreach (var n in data)
        {
            int y = (n - 48) / 3;
            int x = (n - 48) % 3;
            list.Add(new(y, x));
        }

        return list;
    }
}


public static class UserDataMapper
{
    public static UserData ToDataModel(this User user) => new()
    {
        Id = user.Id,
        Login = user.Login,
        Password = user.Password,
        CreatedAt = user.CreatedAt,
        UpdatedAt = user.UpdatedAt
    };

    public static User ToDomainModel(this UserData data) => new()
    {
        Id = data.Id,
        Login = data.Login,
        Password = data.Password,
        CreatedAt = data.CreatedAt,
        UpdatedAt = data.UpdatedAt
    };
}