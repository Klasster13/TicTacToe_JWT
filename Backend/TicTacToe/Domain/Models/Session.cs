using TicTacToe.Common.Enums;
using TicTacToe.Common.Models;

namespace TicTacToe.Domain.Models;

public class Session
{
    public Guid Id { get; set; }
    public Mode Mode { get; set; }
    public Difficulty Difficulty { get; set; }
    public Field Field { get; set; } = null!;
    public State State { get; set; }
    public Guid? Player1Id { get; set; }
    public Guid? Player2Id { get; set; }
    public List<Point>? WinningCells { get; set; }
    public Guid CreatorId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }


    public User? Player1 { get; set; }
    public User? Player2 { get; set; }
}