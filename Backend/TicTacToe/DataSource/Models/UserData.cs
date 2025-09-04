using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TicTacToe.DataSource.Models;

[Table("users")]
public class UserData
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();


    [Required]
    [Column("login", TypeName = "varchar(100)")]
    [StringLength(100, MinimumLength = 3)]
    public string Login { get; set; } = null!;


    [Required]
    [Column("password", TypeName = "varchar(100)")]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = null!;


    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


    // Navigation
    public ICollection<SessionData> AsPlayer1 { get; set; } = [];
    public ICollection<SessionData> AsPlayer2 { get; set; } = [];
}
