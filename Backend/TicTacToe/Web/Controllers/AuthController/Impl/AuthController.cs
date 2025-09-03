using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Domain.Services.AuthService;
using TicTacToe.Web.Mappers;
using TicTacToe.Web.Models.Requests;
using TicTacToe.Web.Models.Responses;

namespace TicTacToe.Web.Controllers.AuthController.Impl;


[Authorize]
[ApiController]
[Route("auth")]
[Produces("application/json")]
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
    /// <param name="request">User login and password</param>
    /// <returns>Access and refresh tokens</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /auth/authorization
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

            var jwtToken = await _authService.AuthorizeUser(request.ToDomainModel());

            if (jwtToken is null)
            {
                return Unauthorized();
            }

            return Ok(jwtToken.ToWebModel());
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
    /// Update access token
    /// </summary>
    /// <param name="request">Refresh token</param>
    /// <returns>Access and refresh tokens</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /auth/refresh-access
    /// </remarks>
    /// <responce code="200">Success</responce>
    /// <responce code="400">Bad ID</responce>
    /// <responce code="401">Unauthorized</responce>
    /// <responce code="404">User not found</responce>
    /// <responce code="500">Internal server error</responce>
    [AllowAnonymous]
    [HttpPost("refresh-access")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JwtResponse>> UpdateAccessToken([FromBody] RefreshJwtRequest request)
    {
        try
        {
            if (request is null || string.IsNullOrEmpty(request.RefreshToken))
            {
                return BadRequest("Provided data is invalid.");
            }

            var token = await _authService.UpdateAccessToken(request.RefreshToken);

            return Ok(token.ToWebModel());
        }
        catch (InvalidOperationException ex)
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
    /// Update tokens
    /// </summary>
    /// <param name="request">Refresh token</param>
    /// <returns>Access and refresh tokens</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /auth/refresh
    /// </remarks>
    /// <responce code="200">Success</responce>
    /// <responce code="400">Bad ID</responce>
    /// <responce code="401">Unauthorized</responce>
    /// <responce code="404">User not found</responce>
    /// <responce code="500">Internal server error</responce>
    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JwtResponse>> UpdateRefreshToken([FromBody] RefreshJwtRequest request)
    {
        try
        {
            if (request is null || string.IsNullOrEmpty(request.RefreshToken))
            {
                return BadRequest("Provided data is invalid.");
            }

            var token = await _authService.UpdateRefreshToken(request.RefreshToken);

            return Ok(token.ToWebModel());
        }
        catch (InvalidOperationException ex)
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
}
