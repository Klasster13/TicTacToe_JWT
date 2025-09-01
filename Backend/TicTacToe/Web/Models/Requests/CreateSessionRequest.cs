using System.ComponentModel.DataAnnotations;
using TicTacToe.Common.Enums;

namespace TicTacToe.Web.Models.Requests;

public class CreateSessionRequest
{
    /// <summary>
    ///  0 - OnePlayer, 1 - TwoPlayers
    /// </summary>
    [Required]
    [AllowedValues([Mode.OnePlayer,
                    Mode.TwoPlayers],
                    ErrorMessage = "Not allowed value for Mode")]
    public Mode Mode { get; set; }


    /// <summary>
    /// Starting side of creator
    /// 0 - Player1/X
    /// 1 - Player2/O
    /// </summary>
    [Required]
    [AllowedValues([Player.Player1,
                    Player.Player2],
                    ErrorMessage = "Not allowed value for StartSide")]
    public Player StartSide { get; set; }


    /// <summary>
    /// Game difficulty
    /// 0 - Easy
    /// 1 - Medium
    /// 2 - Hard
    /// </summary>
    [Required]
    [AllowedValues([Difficulty.Easy,
                    Difficulty.Medium,
                    Difficulty.Hard],
                    ErrorMessage = "Not allowed value for Difficulty")]
    public Difficulty Difficulty { get; set; }
}
