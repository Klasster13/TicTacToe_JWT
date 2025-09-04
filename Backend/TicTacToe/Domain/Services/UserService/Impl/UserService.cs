using TicTacToe.DataSource.Service;
using TicTacToe.Domain.Models;
using TicTacToe.Domain.Services.PasswordService;

namespace TicTacToe.Domain.Services.UserService.Impl;

public class UserService(IServiceProvider serviceProvider,
    IDataService dataService, IPasswordService passwordService) : IUserService
{
    private readonly IDataService _dataService = dataService;
    private readonly IPasswordService _passwordService = passwordService;
    private readonly IServiceProvider _serviceProvider = serviceProvider;


    public async Task<User> CreateUser(User user)
    {
        user.Password = _passwordService.HashPassword(user);
        return await _dataService.AddUser(user);
    }

    public Task<User?> GetUserById(Guid id) => _dataService.GetUserById(id);
    public async Task<User?> GetUserByLogin(string login) => await _dataService.GetUserByLogin(login);
    public async Task DeleteUser(Guid id) => await _dataService.DeleteUser(id);



    public async Task<User> UpdateUser(User updatedUser)
    {
        if (updatedUser.Login.Length < 3 && updatedUser.Password.Length < 6)
        {
            throw new ArgumentException("Bad data for update");
        }

        var savedUser = await _dataService.GetUserById(updatedUser.Id)
            ?? throw new KeyNotFoundException($"Session {updatedUser.Id} not found");

        if (savedUser.Id != updatedUser.Id)
        {
            throw new InvalidOperationException("User is not allowed to modify session");
        }

        if (updatedUser.Login.Length >= 3)
        {
            savedUser.Login = updatedUser.Login;
        }

        if (updatedUser.Password.Length >= 6)
        {
            savedUser.Password = updatedUser.Password;
            savedUser.Password = _passwordService.HashPassword(savedUser);
        }

        await _dataService.UpdateUser(savedUser);
        return savedUser;
    }


    public async Task<IEnumerable<WinRatio>> GetLeaderboard(int limit)
    {
        var users = await _dataService.GetUsers();

        var leaderboard = new List<WinRatio>();

        foreach (var user in users)
        {
            var games = await _dataService.GetFinishedSessions(user.Id);
            int totalGames = games.Count();

            if (totalGames == 0)
            {
                leaderboard.Add(new WinRatio
                {
                    Id = user.Id,
                    Login = user.Login,
                    Ratio = 0
                });
                continue;
            }

            int wins = games.Count(s =>
                (s.Player1Id == user.Id && s.State == Common.Enums.State.Player1Winner) ||
                (s.Player2Id == user.Id && s.State == Common.Enums.State.Player2Winner));

            int loses = totalGames - wins;

            float ratio = loses == 0 ? wins : (float)wins / loses;

            leaderboard.Add(new WinRatio
            {
                Id = user.Id,
                Login = user.Login,
                Ratio = ratio
            });
        }

        return leaderboard
            .OrderByDescending(p => p.Ratio)
            .ThenByDescending(p => p.Login)
            .Take(limit);
    }
}
