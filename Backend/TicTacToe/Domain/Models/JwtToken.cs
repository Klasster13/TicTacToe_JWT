using System.ComponentModel.DataAnnotations;

namespace TicTacToe.Domain.Models;

public class JwtToken
{
    [Required]
    public string AccessToken { get; set; } = null!;


    [Required]
    public string RefreshToken { get; set; } = null!;
}
