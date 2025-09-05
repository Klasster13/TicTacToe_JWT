using Microsoft.AspNetCore.SignalR.Client;
using TicTacToe.Client.Services.TokenStorageService;
using TicTacToe.Client.Models.Responses;
using Blazored.LocalStorage;

namespace TicTacToe.Client.Services.GameHubService;

public class GameHubService(string apiUrl, ITokenStorageService storageService) : IAsyncDisposable
{
    private HubConnection? hubConnection;
    private readonly string apiUrl = CheckApiUrl(apiUrl);
    private bool _disposed;
    private readonly ITokenStorageService _storageService = storageService;

    public Guid SessionId { get; private set; }
    public bool IsConnected { get; private set; } = false;

    public event Action<SessionResponse>? OnUpdateRecieved;
    public event Action<string>? OnErrorRecieved;

    public async Task ConnectAsync(Guid sessionId)
    {
        try
        {
            Console.WriteLine("CONNECT!!!!!");
            hubConnection = new HubConnectionBuilder()
            .WithUrl($"{apiUrl}game/gameHub", options =>
            {
                options.AccessTokenProvider =
                    async () => await _storageService.GetAccessTokenAsync()
                    ?? throw new InvalidOperationException("Can't find access token.");
            })
            .WithAutomaticReconnect()
            .Build();

            hubConnection.On<SessionResponse>("Update",
                session => OnUpdateRecieved?.Invoke(session));

            await hubConnection.StartAsync();

            if (hubConnection.State == HubConnectionState.Connected)
            {
                SessionId = sessionId;
                IsConnected = true;
                await hubConnection.SendAsync("AddClient", sessionId);
            }
        }
        catch (Exception ex)
        {
            OnErrorRecieved?.Invoke($"Connection error: {ex.Message}");
        }
    }


    public async Task DisconnectAsync()
    {
        if (!IsConnected) return;
        if (hubConnection is null) return;

        try
        {
            await hubConnection.SendAsync("DeleteClient", SessionId);
            await hubConnection.StopAsync();
        }
        finally
        {
            await hubConnection.DisposeAsync();
            IsConnected = false;
        }
    }


    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        _disposed = true;

        await DisconnectAsync();
        GC.SuppressFinalize(this);
    }


    private static string CheckApiUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
            return url;

        return url.EndsWith('/') ? url : url + '/';
    }
}
