using System.ComponentModel.DataAnnotations;
using TicTacToe.Common.Enums;

namespace TicTacToe.Web.Models.Responses;

public class FieldWeb
{
    [Required]
    public List<List<Cell>> Board { get; set; } = null!;
}
