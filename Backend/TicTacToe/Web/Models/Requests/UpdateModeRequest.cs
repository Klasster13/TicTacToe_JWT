using System.ComponentModel.DataAnnotations;
using TicTacToe.Common.Enums;

namespace TicTacToe.Web.Models.Requests;

public class UpdateModeRequest
{
    [Required]
    [AllowedValues([Mode.OnePlayer,
                    Mode.TwoPlayers],
                    ErrorMessage = "Not allowed value for Mode")]
    public Mode Mode { get; set; }
}
