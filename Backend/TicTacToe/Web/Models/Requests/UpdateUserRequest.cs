using System.ComponentModel.DataAnnotations;

namespace TicTacToe.Web.Models.Requests;

public class UpdateUserRequest
{
    [StringLength(50, ErrorMessage = "Incorrect login")]
    public string? Login { get; set; }

    [StringLength(100, ErrorMessage = "Incorrect password")]
    public string? Password { get; set; }
}
