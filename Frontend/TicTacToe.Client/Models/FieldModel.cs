using System.ComponentModel.DataAnnotations;
using TicTacToe.Client.Enums;

namespace TicTacToe.Client.Models;

public class FieldModel
{
    [Required]
    public List<List<Cell>> Board { get; set; } = null!;
}
