using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TicTacToe.Common.Enums;

namespace TicTacToe.DataSource.Models;

[Table("sessions")]
public class SessionData
{
    [Required]
    [Key]
    [Column("id")]
    public Guid Id { get; set; }


    [Required]
    [Column("mode")]
    public Mode Mode { get; set; }


    [Required]
    [Column("difficulty")]
    public Difficulty Difficulty { get; set; }


    [Required]
    [Column("field")]
    public string Field { get; set; } = null!;


    [Required]
    [Column("state")]
    public State State { get; set; }


    [Column("player_1_id")]
    public Guid? Player1Id { get; set; }


    [Column("player_2_id")]
    public Guid? Player2Id { get; set; }


    [Column("winning_cells")]
    public string? WinningCells { get; set; }


    [Column("creator_id")]
    public Guid CreatorId { get; set; }


    [Column("created_at")]
    public DateTime CreatedAt { get; set; }


    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }


    // Navigation
    public UserData? Player1 { get; set; }
    public UserData? Player2 { get; set; }
}
