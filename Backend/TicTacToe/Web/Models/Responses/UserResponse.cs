using System.ComponentModel.DataAnnotations;

namespace TicTacToe.Web.Models.Responses;

public class UserResponse
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Login { get; set; } = null!;
}
