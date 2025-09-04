using System.ComponentModel.DataAnnotations;
using TicTacToe.Common.Enums;
using TicTacToe.Domain.Models;

namespace TicTacToe.Web.Models.Requests;

public class MoveRequest
{
    /// <summary>
    /// Updated field
    /// </summary>
    [Required]
    [AllowedCellValues(Cell.X, Cell.O, Cell.None)]
    public List<List<Cell>> Field { get; set; } = null!;
}

public class AllowedCellValuesAttribute(params Cell[] allowedValues) : ValidationAttribute
{
    private readonly Cell[] _allowedValues = allowedValues;

    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        if (value is not List<List<Cell>> field)
            return new ValidationResult("Field must be a list of lists of cells");

        if (field.Count != Field.Size)
            return new ValidationResult($"Row count must be {Field.Size}");

        foreach (var row in field)
        {
            if (row.Count != Field.Size)
                return new ValidationResult($"Column count must be {Field.Size}");

            foreach (var cell in row)
            {
                if (!_allowedValues.Contains(cell))
                {
                    return new ValidationResult(
                        $"Invalid cell value '{cell}'. Allowed values: {string.Join(", ", _allowedValues)}"
                    );
                }
            }
        }

        return ValidationResult.Success;
    }
}