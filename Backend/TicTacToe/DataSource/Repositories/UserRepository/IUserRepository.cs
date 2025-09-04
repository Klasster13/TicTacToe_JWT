using TicTacToe.Domain.Models;

namespace TicTacToe.DataSource.Repositories.UserRepository;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByLoginAsync(string login);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> AddAsync(User user);
    Task DeleteAsync(Guid id);
    Task<User> UpdateAsync(User user);
}
