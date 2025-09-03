using System.ComponentModel.DataAnnotations;

namespace TicTacToe.Client.Models.Responses;

public class JwtResponse
{
    [Required]
    public string AccessToken { get; set; } = null!;


    [Required]
    public string RefreshToken { get; set; } = null!;
}
