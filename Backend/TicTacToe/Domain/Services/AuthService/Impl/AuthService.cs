using System.Security.Claims;
using TicTacToe.Domain.Models;
using TicTacToe.Domain.Services.JwtService;
using TicTacToe.Domain.Services.PasswordService;
using TicTacToe.Domain.Services.UserService;

namespace TicTacToe.Domain.Services.AuthService.Impl;

public class AuthService(
    IUserService userService,
    IPasswordService passwordService,
    IJwtService jwtService
    ) : IAuthService
{
    private readonly IUserService _userService = userService;
    private readonly IPasswordService _passwordService = passwordService;
    private readonly IJwtService _jwtService = jwtService;

    public async Task<User> RegisterUser(User user) => await _userService.CreateUser(user);


    public async Task<JwtToken?> AuthorizeUser(User user)
    {
        var existingUser = await _userService.GetUserByLogin(user.Login);

        if (existingUser is null)
        {
            return null;
        }

        if (!_passwordService.Verify(existingUser, user.Password))
        {
            return null;
        }

        return new JwtToken
        {
            AccessToken = _jwtService.GenerateAccessToken(existingUser),
            RefreshToken = _jwtService.GenerateRefreshToken(existingUser)
        };
    }


    public async Task<JwtToken> UpdateAccessToken(string refreshToken)
    {
        if (!_jwtService.ValidateRefreshToken(refreshToken))
        {
            throw new InvalidOperationException("Refresh token is invalid.");
        }

        var claims = _jwtService.GetClaims(refreshToken);
        var userIdString = claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            throw new InvalidOperationException("Claims is invalid.");
        }

        var user = await _userService.GetUserById(userId)
                ?? throw new KeyNotFoundException("User not found.");

        return new JwtToken
        {
            AccessToken = _jwtService.GenerateAccessToken(user),
            RefreshToken = refreshToken
        };
    }


    public async Task<JwtToken> UpdateRefreshToken(string refreshToken)
    {
        if (!_jwtService.ValidateRefreshToken(refreshToken))
        {
            throw new InvalidOperationException("Refresh token is invalid.");
        }

        var claims = _jwtService.GetClaims(refreshToken);
        var userIdString = claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            throw new InvalidOperationException("Claims is invalid.");
        }

        var user = await _userService.GetUserById(userId)
                ?? throw new KeyNotFoundException("User not found.");

        return new JwtToken
        {
            AccessToken = _jwtService.GenerateAccessToken(user),
            RefreshToken = _jwtService.GenerateRefreshToken(user)
        };
    }
}
