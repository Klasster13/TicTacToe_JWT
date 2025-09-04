using System.ComponentModel.DataAnnotations;

namespace TicTacToe.Domain.Models;

public class WinRatio
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Login { get; set; } = null!;

    [Required]
    public float Ratio { get; set; }
}
