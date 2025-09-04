using Microsoft.AspNetCore.Identity;
using TicTacToe.Domain.Models;

namespace TicTacToe.Domain.Services.PasswordService.Impl;

public class PasswordService : IPasswordService
{
    private readonly PasswordHasher<User> _passwordHasher = new();

    public string HashPassword(User user)
    {
        if (user is null || string.IsNullOrEmpty(user.Password))
        {
            throw new ArgumentNullException(nameof(user));
        }

        return _passwordHasher.HashPassword(user, user.Password);
    }

    public bool Verify(User user, string providedPassword)
    {
        if (string.IsNullOrEmpty(providedPassword))
        {
            return false;
        }

        if (user is null || string.IsNullOrEmpty(user.Password))
        {
            throw new ArgumentNullException(nameof(user));
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, providedPassword);
        return result != PasswordVerificationResult.Failed;
    }
}
