using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Domain.Services.AuthService;
using TicTacToe.Web.Mappers;
using TicTacToe.Web.Models.Requests;
using TicTacToe.Web.Models.Responses;

namespace TicTacToe.Web.Controllers.AuthController.Impl;


[ApiController]
[Route("auth")]
[Produces("application/json")]
[Authorize]
public class AuthController(IAuthService authService) : ControllerBase, IAuthController
{
    private readonly IAuthService _authService = authService;


    /// <summary>
    /// Register new user
    /// </summary>
    /// <param name="signUpRequest"></param>
    /// <returns>Newly created user</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /auth/registration
    ///     {
    ///         "login":"yourlogin",
    ///         "password":"yourpassword"
    ///     }
    /// </remarks>
    /// <responce code="201">Delete was successful</responce>
    /// <responce code="400">Bad data</responce>
    /// <responce code="500">Internal server error</responce>
    [AllowAnonymous]
    [HttpPost("registration")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> Registration([FromBody] SignUpRequest signUpRequest)
    {
        try
        {
            var createdUser = await _authService.RegisterUser(signUpRequest.ToDomainModel());

            return CreatedAtAction(
                actionName: "GetUserById",
                controllerName: "User",
                routeValues: new { userId = createdUser.Id },
                value: createdUser.ToWebModel()
            );


        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error");
        }
    }


    /// <summary>
    /// Authorize existing user
    /// </summary>
    /// <returns>No content</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /game/a3d4e5f6-7890-1234-b567-c89012345678
    /// </remarks>
    /// <responce code="200">Success</responce>
    /// <responce code="400">Bad ID</responce>
    /// <responce code="401">Unauthorized</responce>
    /// <responce code="500">Internal server error</responce>
    [AllowAnonymous]
    [HttpGet("authorization")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JwtResponse>> Authorization([FromBody] JwtRequest request)
    {
        try
        {
            if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                return Unauthorized();
            }

            var existingUser = await _authService.AuthorizeUser(request.ToDomainModel());

            if (existingUser is null)
            {
                return Unauthorized();
            }

            var accessToken =
            return Ok(id);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error");
        }
    }


    public Task<ActionResult<JwtResponse>> UpdateAccessToken(RefreshJwtRequest request)
    {
        throw new NotImplementedException();
    }


    public Task<ActionResult<JwtResponse>> UpdateRefreshToken(RefreshJwtRequest request)
    {
        throw new NotImplementedException();
    }
}
