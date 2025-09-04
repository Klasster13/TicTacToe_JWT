using System.ComponentModel.DataAnnotations;

namespace TicTacToe.Client.Models.Responses;

public class WinRatioResponse
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Login { get; set; } = null!;

    [Required]
    public float WinRatio { get; set; }
}
