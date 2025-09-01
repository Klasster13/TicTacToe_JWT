using Microsoft.AspNetCore.Mvc;
using TicTacToe.Web.Models.Requests;
using TicTacToe.Web.Models.Responses;

namespace TicTacToe.Web.Controllers.UserController;

public interface IUserController
{
    Task<ActionResult<UserResponse>> GetUserById(Guid id);
    Task<IActionResult> UpdateUser(UpdateUserRequest request);
}
