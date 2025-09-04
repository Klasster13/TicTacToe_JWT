using TicTacToe.DataSource.Models;

namespace TicTacToe.Domain.Models;

public class User
{
    public Guid Id { get; set; }
    public string Login { get; set; } = null!;
    public string Password { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<Session>? AsPlayer1 { get; set; }
    public ICollection<Session>? AsPlayer2 { get; set; }
}
