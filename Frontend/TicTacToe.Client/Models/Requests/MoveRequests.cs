using System.ComponentModel.DataAnnotations;
using TicTacToe.Client.Enums;

namespace TicTacToe.Client.Models.Requests;

public class MoveRequests
{
    [Required]
    public List<List<Cell>> Field { get; set; } = null!;
}
