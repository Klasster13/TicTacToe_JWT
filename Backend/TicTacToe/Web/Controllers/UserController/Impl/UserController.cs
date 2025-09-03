using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Domain.Services.UserService;
using TicTacToe.Web.Mappers;
using TicTacToe.Web.Models.Requests;
using TicTacToe.Web.Models.Responses;

namespace TicTacToe.Web.Controllers.UserController.Impl;

[ApiController]
[Route("user")]
[Produces("application/json")]
[Authorize]
public class UserController(IUserService userService) : ControllerBase, IUserController
{
    private readonly IUserService _userService = userService;


    /// <summary>
    /// Get user by ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>User data</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /user/a3d4e5f6-7890-1234-b567-c89012345678
    /// </remarks>
    /// <responce code="200">Success</responce>
    /// <responce code="400">Bad data</responce>
    /// <responce code="401">Unauthorized</responce>
    /// <responce code="404">Session was not found</responce>
    /// <responce code="500">Internal server error</responce>
    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserResponse>> GetUserById(Guid userId)
    {
        try
        {
            var user = await _userService.GetUserById(userId);

            if (user is null)
            {
                return NotFound($"User {userId} not found");
            }

            return Ok(user.ToWebModel());
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error");
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// Sample request:
    /// 
    ///     PUT /user/update
    ///     {
    ///       "login":"newlogin",
    ///       "password":"newpassword"  
    ///     }
    /// </remarks>
    /// <responce code="200">Success</responce>
    /// <responce code="400">Bad data</responce>
    /// <responce code="401">Unauthorized</responce>
    /// <responce code="404">User was not found</responce>
    /// <responce code="409">User is not allwed to modify session</responce>
    /// <responce code="500">Internal server error</responce>
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
    {
        try
        {
            if (!HttpContext.Items.TryGetValue("UserId", out var userIdObj) || userIdObj is null)
            {
                return Unauthorized();
            }
            var userId = (Guid)userIdObj;

            if ((request.Login is null || request.Login.Length < 3)
                && (request.Password is null || request.Password.Length < 6))
            {
                return BadRequest("Bad data for update");
            }

            var updatedUser = await _userService.UpdateUser(request.ToDomainModel(userId));

            return Ok(updatedUser.ToWebModel());
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
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
    /// Get current user
    /// </summary>
    /// <returns>User data</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /user/current
    /// </remarks>
    /// <responce code="200">Success</responce>
    /// <responce code="400">Bad data</responce>
    /// <responce code="401">Unauthorized</responce>
    /// <responce code="404">Session was not found</responce>
    /// <responce code="500">Internal server error</responce>
    [HttpGet("current")]
    public async Task<ActionResult<UserResponse>> GetCurrentUser()
    {
        try
        {
            var login = HttpContext.User.Identity?.Name;

            if (string.IsNullOrEmpty(login))
            {
                return BadRequest("Provided data is invalid.");
            }

            var user = await _userService.GetUserByLogin(login);

            if (user is null)
            {
                return NotFound("User not found");
            }

            return Ok(user.ToWebModel());
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error");
        }
    }
}
