using TicTacToe.Client.Models.Responses;

namespace TicTacToe.Client.Services;

public class UserDataUpdateService
{
    public event Action<UserResponse>? OnUserUpdate;

    public void UserDataChanged(UserResponse user) => OnUserUpdate?.Invoke(user);
}
