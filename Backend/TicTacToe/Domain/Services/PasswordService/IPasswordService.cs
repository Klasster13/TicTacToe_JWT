using TicTacToe.Domain.Models;

namespace TicTacToe.Domain.Services.PasswordService;

public interface IPasswordService
{
    string HashPassword(User user);
    bool Verify(User user, string providedPassword);
}
