using System.ComponentModel.DataAnnotations;
using TicTacToe.Client.Enums;

namespace TicTacToe.Client.Models.Requests;

public class UpdateModeRequest
{
    [Required]
    [AllowedValues([Mode.OnePlayer,
                    Mode.TwoPlayers],
                    ErrorMessage = "Not allowed value for Mode")]
    public Mode Mode { get; set; }
}
