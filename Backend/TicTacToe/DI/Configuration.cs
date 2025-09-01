using TicTacToe.DataSource.Service;
using TicTacToe.DataSource.Service.Impl;
using TicTacToe.Domain.Services.GameService;
using TicTacToe.Domain.Services.GameService.Impl;
using TicTacToe.Web.Controllers.GameController.Impl;
using TicTacToe.Web.Controllers.GameController;
using TicTacToe.Domain.Services.UserService;
using TicTacToe.Domain.Services.UserService.Impl;
using TicTacToe.DataSource.Repositories.UserRepository;
using TicTacToe.DataSource.Repositories.UserRepository.Impl;
using TicTacToe.DataSource.Repositories.SessionRepository.Impl;
using TicTacToe.DataSource.Repositories.SessionRepository;
using TicTacToe.Web.Controllers.AuthController;
using TicTacToe.Web.Controllers.AuthController.Impl;
using TicTacToe.Web.Controllers.UserController;
using TicTacToe.Web.Controllers.UserController.Impl;
using TicTacToe.Domain.Services.AuthService;
using TicTacToe.Domain.Services.AuthService.Impl;
using TicTacToe.Web.Filter;
using TicTacToe.Domain.Services.PasswordService;
using TicTacToe.Domain.Services.PasswordService.Impl;

namespace TicTacToe.DI;

public static class DependencyConfiguration
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IDataService, DataService>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<IUserService, UserService>();
        services.AddSingleton<IPasswordService, PasswordService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IGameController, GameController>();
        services.AddScoped<IAuthController, AuthController>();
        services.AddScoped<IUserController, UserController>();
        services.AddScoped<AuthFilter>();

        return services;
    }
}