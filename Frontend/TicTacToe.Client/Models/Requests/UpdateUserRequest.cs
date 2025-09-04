using System.ComponentModel.DataAnnotations;

namespace TicTacToe.Client.Models.Requests;

public class UpdateUserRequest
{
    [StringLength(50, ErrorMessage = "Incorrect login", MinimumLength = 3)]
    public string? Login { get; set; }

    [StringLength(100, ErrorMessage = "Incorrect password", MinimumLength = 6)]
    public string? Password { get; set; }
}
