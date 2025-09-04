using System.ComponentModel.DataAnnotations;

namespace TicTacToe.Web.Models.Requests;

public class SignUpRequest
{
    [Required]
    [StringLength(50, ErrorMessage = "Incorrect login", MinimumLength = 3)]
    public string Login { get; set; } = null!;

    [Required]
    [StringLength(100, ErrorMessage = "Incorrect password", MinimumLength = 6)]
    public string Password { get; set; } = null!;
}
