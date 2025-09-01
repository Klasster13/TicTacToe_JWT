using TicTacToe.DataSource.Repositories.SessionRepository;
using TicTacToe.DataSource.Repositories.UserRepository;
using TicTacToe.Domain.Models;

namespace TicTacToe.DataSource.Service.Impl;

public class DataService(IUserRepository userRepository, ISessionRepository sessionRepository) : IDataService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ISessionRepository _sessionRepository = sessionRepository;

    public async Task<Session?> GetSession(Guid sessionId) => await _sessionRepository.GetByIdAsync(sessionId);
    public async Task<IEnumerable<Session>> GetAvailableSessions(Guid userId) => await _sessionRepository.GetAvailableSessionsAsync(userId);
    public async Task<IEnumerable<Session>> GetUserSessions(Guid userId) => await _sessionRepository.GetUserSessionsAsync(userId);
    public async Task AddSession(Session session) => await _sessionRepository.AddAsync(session);
    public async Task UpdateSession(Session session) => await _sessionRepository.UpdateAsync(session);
    public async Task DeleteSession(Guid sessionId) => await _sessionRepository.DeleteAsync(sessionId);


    public async Task<User?> GetUserById(Guid userId) => await _userRepository.GetByIdAsync(userId);
    public async Task<User?> GetUserByLogin(string login) => await _userRepository.GetByLoginAsync(login);
    public async Task<IEnumerable<User>> GetUsers() => await _userRepository.GetAllAsync();
    public async Task<User> AddUser(User user) => await _userRepository.AddAsync(user);
    public async Task DeleteUser(Guid userId) => await _userRepository.DeleteAsync(userId);
    public async Task<User> UpdateUser(User user) => await _userRepository.UpdateAsync(user);


}