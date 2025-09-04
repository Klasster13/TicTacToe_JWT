using TicTacToe.Domain.Models;

namespace TicTacToe.DataSource.Repositories.SessionRepository;

public interface ISessionRepository
{
    Task<Session?> GetByIdAsync(Guid id);
    //Task<IEnumerable<Session>> GetAllAsync();
    Task<IEnumerable<Session>> GetAvailableSessionsAsync(Guid userId);
    Task<IEnumerable<Session>> GetUserSessionsAsync(Guid userId);
    Task<IEnumerable<Session>> GetFinishedSessions(Guid userId);
    Task AddAsync(Session session);
    Task UpdateAsync(Session session);
    Task DeleteAsync(Guid id);
}
