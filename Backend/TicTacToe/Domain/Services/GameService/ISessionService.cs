using TicTacToe.Common.Enums;
using TicTacToe.Domain.Models;

namespace TicTacToe.Domain.Services.GameService;

public interface ISessionService
{
    Task<Session> CreateSession(Session session);
    Task<Session?> GetSession(Guid sessionId);
    Task<Session?> AddPlayerToSession(Guid sessionId, Guid userId);
    Task<IEnumerable<Session>> GetAvailableSessions(Guid userId);
    Task<IEnumerable<Session>> GetUserSessions(Guid userId);
    Task DeleteSession(Guid sessionId);
    Task<Session?> ValidateMove(Guid userId, Guid sessionId, Field newField);
    Task<Session> MakeMove(Session session);
    Task<Session> UpdateSessionMode(Guid sessionId, Guid userId, Mode newMode);
    Task<Session> ResetSession(Guid sessionId, Guid userId);
    Task<IEnumerable<Session>> GetFinishedSessions(Guid userId);
}
