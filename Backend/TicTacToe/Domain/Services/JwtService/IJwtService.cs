using System.Security.Claims;
using TicTacToe.Domain.Models;

namespace TicTacToe.Domain.Services.JwtService;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken(User user);
    bool ValidateAccessToken(string accessToken);
    bool ValidateRefreshToken(string refreshToken);
    ClaimsPrincipal GetClaims(string token);
}
