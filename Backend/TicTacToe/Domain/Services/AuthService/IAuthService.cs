using TicTacToe.Domain.Models;

namespace TicTacToe.Domain.Services.AuthService;

public interface IAuthService
{
    Task<User> RegisterUser(User user);
    Task<JwtToken?> AuthorizeUser(User user);
    Task<JwtToken?> UpdateAccessToken(string refreshToken);
    Task<JwtToken?> UpdateRefreshToken(string refreshToken);
}
