using System.ComponentModel.DataAnnotations;

namespace TicTacToe.Client.Models.Requests;

public class RefreshJwtRequest
{
    [Required]
    public string RefreshToken { get; set; } = null!;
}
