using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TicTacToe.Domain.Services.GameService;
using TicTacToe.Web.Filter;
using TicTacToe.Web.Mappers;
using TicTacToe.Web.Models.Requests;
using TicTacToe.Web.Models.Responses;
using TicTacToe.Web.SignalRHub;

namespace TicTacToe.Web.Controllers.GameController.Impl;


[ApiController]
[Route("game")]
[Produces("application/json")]
[ServiceFilter(typeof(AuthFilter))]
public class GameController(ISessionService sessionService, IHubContext<GameHub> hub) : ControllerBase, IGameController
{
    private readonly ISessionService _sessionService = sessionService;
    private readonly IHubContext<GameHub> _hub = hub;


    /// <summary>
    /// Get session by id
    /// </summary>
    /// <param name="sessionId">Session ID</param>
    /// <returns>Session by ID</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /game/a3d4e5f6-7890-1234-b567-c89012345678
    /// </remarks>
    /// <responce code="200">Success</responce>
    /// <responce code="400">Bad ID</responce>
    /// <responce code="404">Session with id was not found</responce>
    [HttpGet("{sessionId}")]
    [ProducesResponseType(typeof(SessionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SessionResponse>> GetSession(Guid sessionId)
    {
        try
        {
            var session = await _sessionService.GetSession(sessionId);
            if (session is null)
                return NotFound($"Game session {sessionId} not found");

            return Ok(session.ToWebModel());
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }


    /// <summary>
    /// Create new session
    /// </summary>
    /// <param name="request">New data</param>
    /// <returns>Newly created session data</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /game/
    ///     {
    ///         "mode":0,
    ///         "startSide":0
    ///     } 
    /// </remarks>
    /// <responce code="201">Success</responce>
    /// <responce code="400">Bad data</responce>
    /// <responce code="401">Unauthorized</responce>
    /// <responce code="500">Internal server error</responce>
    [HttpPost("create")]
    [ProducesResponseType(typeof(SessionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SessionResponse>> CreateSession([FromBody] CreateSessionRequest request)
    {
        try
        {
            if (!HttpContext.Items.TryGetValue("UserId", out var userIdObj) || userIdObj is null)
            {
                return Unauthorized();
            }

            var userId = (Guid)userIdObj;

            var newSession = request.ToDomainModel(userId);
            var createdSession = await _sessionService.CreateSession(newSession);

            if (createdSession is null)
                return StatusCode(500, "Internal server error");

            return CreatedAtAction(
                actionName: nameof(GetSession),
                routeValues: new { sessionId = createdSession.Id },
                value: createdSession.ToWebModel());
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }


    /// <summary>
    /// Making next move in TicTacToe game
    /// </summary>
    /// <param name="sessionId">Session id</param>
    /// <param name="request">Game data</param>
    /// <returns>Updated session after moving</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /game/88079ab6-3ff3-45e9-8adc-5965f3f3ee77
    ///     {
    ///         "field":[[0,0,0],[0,1,0],[0,0,0]]
    ///     }
    /// </remarks>
    /// <responce code="200">Success</responce>
    /// <responce code="400">Bad data</responce>
    /// <responce code="401">Unauthorized</responce>
    /// <responce code="404">Session was not found</responce>
    /// <responce code="409">User is not allwed to modify session</responce>
    /// <responce code="500">Internal server error</responce>
    [HttpPut("{sessionId}/move")]
    [ProducesResponseType(typeof(SessionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SessionResponse>> MakeMove(Guid sessionId, [FromBody] MoveRequest request)
    {
        try
        {
            if (request?.Field is null)
            {
                return BadRequest("Data is corrupted.");
            }

            if (Guid.Empty == sessionId)
            {
                return BadRequest("Bad id");
            }

            if (!HttpContext.Items.TryGetValue("UserId", out var userIdObj) || userIdObj is null)
            {
                return Unauthorized();
            }
            var userId = (Guid)userIdObj;
            var newField = request.ToDomainModel();

            var validSession = await _sessionService.ValidateMove(userId, sessionId, newField);
            if (validSession is null)
            {
                return BadRequest("Field data mismatch");
            }

            var updatedSession = await _sessionService.MakeMove(validSession);

            var response = updatedSession.ToWebModel();

            if (response.Mode == Common.Enums.Mode.TwoPlayers)
            {
                await _hub.Clients.Group(response.Id.ToString())
                    .SendAsync("Update", response);
            }

            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex) when (ex is ArgumentException || ex is ArgumentOutOfRangeException)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error");
        }
    }


    /// <summary>
    /// Delete session by ID
    /// </summary>
    /// <param name="sessionId">Session ID</param>
    /// <returns>No content</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     DELETE /game/a3d4e5f6-7890-1234-b567-c89012345678
    /// </remarks>
    /// <responce code="204">Success</responce>
    /// <responce code="400">Bad data</responce>
    /// <responce code="401">Unauthorized</responce>
    /// <responce code="404">Session was not found</responce>
    /// <responce code="409">User is not allwed to modify session</responce>
    /// <responce code="500">Internal server error</responce>
    [HttpDelete("{sessionId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteSession(Guid sessionId)
    {
        try
        {
            if (!HttpContext.Items.TryGetValue("UserId", out var userIdObj) || userIdObj is null)
            {
                return Unauthorized();
            }
            var userId = (Guid)userIdObj;

            var session = await _sessionService.GetSession(sessionId)
                ?? throw new KeyNotFoundException($"Session {sessionId} not found");

            if (session.CreatorId != userId)
            {
                return Conflict("User is not allowed to modify this session");
            }

            await _sessionService.DeleteSession(sessionId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error");
        }
    }


    /// <summary>
    /// Allow user to join session by ID
    /// </summary>
    /// <param name="sessionId"></param>
    /// <returns>Updated session with joined user</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /game/a3d4e5f6-7890-1234-b567-c89012345678/join
    /// </remarks>
    /// <responce code="200">Success</responce>
    /// <responce code="400">Bad data</responce>
    /// <responce code="401">Unauthorized</responce>
    /// <responce code="404">Session was not found</responce>
    /// <responce code="409">User is not allwed to modify session</responce>
    /// <responce code="500">Internal server error</responce>
    [HttpGet("{sessionId}/join")]
    [ProducesResponseType(typeof(SessionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SessionResponse>> AddPlayerToSession(Guid sessionId)
    {
        try
        {
            if (!HttpContext.Items.TryGetValue("UserId", out var userIdObj) || userIdObj is null)
            {
                return Unauthorized();
            }
            var userId = (Guid)userIdObj;

            var session = await _sessionService.AddPlayerToSession(sessionId, userId);

            if (session is null)
            {
                return NotFound($"Session {sessionId} not found");
            }

            var response = session.ToWebModel();

            await _hub.Clients.Group(response.Id.ToString())
                    .SendAsync("Update", response);

            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error");
        }
    }


    /// <summary>
    /// Get all sessions available for join
    /// </summary>
    /// <returns>List of available sessions</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /game/available
    /// </remarks>
    /// <responce code="200">Success</responce>
    /// <responce code="401">Unauthorized</responce>
    /// <responce code="500">Internal server error</responce>
    [HttpGet("available")]
    [ProducesResponseType(typeof(IEnumerable<SessionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<SessionResponse>>> GetAvailableSessions()
    {
        try
        {
            if (!HttpContext.Items.TryGetValue("UserId", out var userIdObj) || userIdObj is null)
            {
                return Unauthorized();
            }
            var userId = (Guid)userIdObj;

            var sessions = await _sessionService.GetAvailableSessions(userId);
            return Ok(sessions.Select(s => s.ToWebModel()));
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error");
        }
    }


    /// <summary>
    /// Get user's current sessions
    /// </summary>
    /// <returns>List of user's sessions</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /game/my-games
    /// </remarks>
    /// <responce code="200">Delete was successful</responce>
    /// <responce code="401">Unauthorized</responce>
    /// <responce code="500">Internal server error</responce>
    [HttpGet("my-games")]
    [ProducesResponseType(typeof(IEnumerable<SessionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<SessionResponse>>> GetUserSessions()
    {
        try
        {
            if (!HttpContext.Items.TryGetValue("UserId", out var userIdObj) || userIdObj is null)
            {
                return Unauthorized();
            }

            var userId = (Guid)userIdObj;

            var sessions = await _sessionService.GetUserSessions(userId);
            return Ok(sessions.Select(s => s.ToWebModel()));
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error");
        }
    }


    /// <summary>
    /// Change session mode
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="request"></param>
    /// <returns>Updated session</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /game/a3d4e5f6-7890-1234-b567-c89012345678/mode
    ///     {
    ///         "mode":1
    ///     }
    /// </remarks>
    /// <responce code="200">Success</responce>
    /// <responce code="400">Bad data</responce>
    /// <responce code="401">Unauthorized</responce>
    /// <responce code="404">Session was not found</responce>
    /// <responce code="409">User is not allwed to modify session</responce>
    /// <responce code="500">Internal server error</responce>
    [HttpPut("{sessionId}/mode")]
    [ProducesResponseType(typeof(SessionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SessionResponse>> UpdateMode(Guid sessionId, [FromBody] UpdateModeRequest request)
    {
        try
        {
            if (!HttpContext.Items.TryGetValue("UserId", out var userIdObj) || userIdObj is null)
            {
                return Unauthorized();
            }
            var userId = (Guid)userIdObj;

            var updatedSession = await _sessionService.UpdateSessionMode(sessionId, userId, request.Mode);

            return Ok(updatedSession.ToWebModel());
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error");
        }
    }


    /// <summary>
    /// Reset session
    /// </summary>
    /// <param name="sessionId">Session ID</param>
    /// <returns>Session after reset</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /game/a3d4e5f6-7890-1234-b567-c89012345678/reset
    /// </remarks>
    /// <responce code="200">Success</responce>
    /// <responce code="401">Unauthorized</responce>
    /// <responce code="404">Session was not found</responce>
    /// <responce code="409">User is not allwed to modify session</responce>
    /// <responce code="500">Internal server error</responce>
    [HttpGet("{sessionId}/reset")]
    [ProducesResponseType(typeof(SessionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SessionResponse>> ResetSession(Guid sessionId)
    {
        try
        {
            if (!HttpContext.Items.TryGetValue("UserId", out var userIdObj) || userIdObj is null)
            {
                return Unauthorized();
            }
            var userId = (Guid)userIdObj;

            var resetedSession = await _sessionService.ResetSession(sessionId, userId);

            var response = resetedSession.ToWebModel();

            if (response.Mode == Common.Enums.Mode.TwoPlayers)
            {
                await _hub.Clients.Group(response.Id.ToString())
                    .SendAsync("Update", response);
            }

            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error");
        }
    }
}
