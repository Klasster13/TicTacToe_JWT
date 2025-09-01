using Microsoft.AspNetCore.Mvc;
using TicTacToe.Domain.Models;
using TicTacToe.Web.Models.Requests;
using TicTacToe.Web.Models.Responses;

namespace TicTacToe.Web.Controllers.AuthController;

public interface IAuthController
{
    Task<ActionResult<UserResponse>> Registration(SignUpRequest signUpRequest);
    Task<IActionResult> Authorization();
}
