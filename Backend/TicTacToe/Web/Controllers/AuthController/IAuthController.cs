using Microsoft.AspNetCore.Mvc;
using TicTacToe.Web.Models.Requests;
using TicTacToe.Web.Models.Responses;

namespace TicTacToe.Web.Controllers.AuthController;

public interface IAuthController
{
    Task<ActionResult<UserResponse>> Registration(SignUpRequest signUpRequest);
    Task<ActionResult<JwtResponse>> Authorization(JwtRequest request);
    Task<ActionResult<JwtResponse>> UpdateAccessToken(RefreshJwtRequest request);
    Task<ActionResult<JwtResponse>> UpdateRefreshToken(RefreshJwtRequest request);
}
