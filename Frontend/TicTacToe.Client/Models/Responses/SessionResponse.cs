using System.ComponentModel.DataAnnotations;
using TicTacToe.Client.Enums;

namespace TicTacToe.Client.Models.Responses;

public class SessionResponse
{
    /// <summary>
    /// Game ID, can't be empty or null
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    ///  0 - OnePlayer,
    ///  1 - TwoPlayers
    /// </summary>
    [Required]
    [AllowedValues([Mode.OnePlayer,
                    Mode.TwoPlayers],
                    ErrorMessage = "Not allowed value for Mode")]
    public Mode Mode { get; set; }


    /// <summary>
    /// Game field
    /// </summary>
    [Required]
    public FieldModel Field { get; set; } = null!;


    /// <summary>
    /// 0 - WaitingForPlayers,
    /// 1 - Player1Turn,
    /// 2 - Player2Turn,
    /// 3 - Draw,
    /// 4 - X win,
    /// 5 - O win
    /// </summary>
    [AllowedValues([State.WaitingForPlayers,
                    State.Player1Turn,
                    State.Player2Turn,
                    State.Draw,
                    State.Player1Winner,
                    State.Player2Winner,
                    null],
                    ErrorMessage = "Not allowed value for GameState")]
    [Required]
    public State State { get; set; }


    /// <summary>
    /// Player 1 ID
    /// </summary>
    public Guid? Player1Id { get; set; }


    /// <summary>
    /// Player 2 ID
    /// </summary>
    public Guid? Player2Id { get; set; }


    /// <summary>
    /// Can be null, exxist only if GameState != 0
    /// </summary>
    public ICollection<PointModel>? WinningCells { get; set; }


    /// <summary>
    /// User ID, who created session
    /// </summary>
    public Guid CreatorId { get; set; }
}
