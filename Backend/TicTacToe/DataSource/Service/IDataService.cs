using TicTacToe.Domain.Models;

namespace TicTacToe.DataSource.Service;

public interface IDataService
{
    Task<Session?> GetSession(Guid id);
    Task<IEnumerable<Session>> GetUserSessions(Guid userId);
    Task<IEnumerable<Session>> GetAvailableSessions(Guid userId);
    Task AddSession(Session session);
    Task UpdateSession(Session session);
    Task DeleteSession(Guid id);


    Task<User?> GetUserById(Guid id);
    Task<User?> GetUserByLogin(string login);
    Task<IEnumerable<User>> GetUsers();
    Task<User> AddUser(User user);
    Task DeleteUser(Guid id);
    Task<User> UpdateUser(User user);
}
