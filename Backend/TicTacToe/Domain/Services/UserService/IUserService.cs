using TicTacToe.Domain.Models;

namespace TicTacToe.Domain.Services.UserService;

public interface IUserService
{
    Task<User> CreateUser(User user);
    Task<User?> GetUserById(Guid id);
    Task<User?> GetUserByLogin(string login);
    Task DeleteUser(Guid id);
    Task<User> UpdateUser(User updatedUser);
    Task<IEnumerable<WinRatio>> GetLeaderboard(int limit);
}
