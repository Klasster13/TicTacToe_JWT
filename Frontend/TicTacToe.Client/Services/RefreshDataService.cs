namespace TicTacToe.Client.Services;

public class RefreshDataService
{
    public event Func<Task>? OnRefreshDataAsync;

    public async Task RefreshDataAsync()
    {
        if (OnRefreshDataAsync is null) return;

        await OnRefreshDataAsync.Invoke();
    }
}
