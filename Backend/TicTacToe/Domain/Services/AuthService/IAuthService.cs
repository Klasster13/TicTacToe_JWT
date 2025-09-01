using TicTacToe.Domain.Models;

namespace TicTacToe.Domain.Services.AuthService;

public interface IAuthService
{
    Task<User> RegisterUser(User user);
    Task<Guid?> AuthorizeUser(User user);
    User? GetUserFromBase64(string authHeader);
}
