using Microsoft.AspNetCore.Mvc;
using TicTacToe.Domain.Models;
using TicTacToe.Web.Models.Requests;
using TicTacToe.Web.Models.Responses;

namespace TicTacToe.Web.Controllers.GameController;

public interface IGameController
{
    Task<ActionResult<SessionResponse>> CreateSession(CreateSessionRequest request);
    Task<IActionResult> DeleteSession(Guid sessionId);
    Task<ActionResult<SessionResponse>> GetSession(Guid sessionId);
    Task<ActionResult<SessionResponse>> MakeMove(Guid sessionId, MoveRequest request);
    Task<ActionResult<SessionResponse>> AddPlayerToSession(Guid sessionId);
    Task<ActionResult<IEnumerable<SessionResponse>>> GetAvailableSessions();
    Task<ActionResult<IEnumerable<SessionResponse>>> GetUserSessions();
    Task<ActionResult<SessionResponse>> UpdateMode(Guid sessionId, UpdateModeRequest request);
    Task<ActionResult<SessionResponse>> ResetSession(Guid sessionId);
    Task<ActionResult<IEnumerable<SessionResponse>>> GetFinishedSessions();
}
