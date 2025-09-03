using System.ComponentModel.DataAnnotations;

namespace TicTacToe.Web.Models.Requests;

public class RefreshJwtRequest
{
    [Required]
    public string RefreshToken { get; set; } = null!;
}
