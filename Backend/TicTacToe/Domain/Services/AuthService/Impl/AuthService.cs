using System.Text;
using TicTacToe.Domain.Models;
using TicTacToe.Domain.Services.PasswordService;
using TicTacToe.Domain.Services.UserService;

namespace TicTacToe.Domain.Services.AuthService.Impl;

public class AuthService(IUserService userService, IPasswordService passwordService) : IAuthService
{
    private readonly IUserService _userService = userService;
    private readonly IPasswordService _passwordService = passwordService;


    public async Task<User> RegisterUser(User user) => await _userService.CreateUser(user);


    public async Task<Guid?> AuthorizeUser(User user)
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

        return existingUser.Id;
    }


    public User? GetUserFromBase64(string authHeader)
    {
        var authString = authHeader.ToString();

        if (string.IsNullOrEmpty(authString) || !authString.StartsWith("Basic "))
        {
            return null;
        }

        var encodedString = authString["Basic ".Length..];
        var decodedString = Encoding.UTF8.GetString(Convert.FromBase64String(encodedString));

        var data = decodedString.Split(':', 2);

        if (data.Length != 2)
        {
            return null;
        }

        var user = new User
        {
            Login = data[0],
            Password = data[1]
        };

        return user;
    }
}
