using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TicTacToe.Web.Filter;

namespace TicTacToe.Web.SignalRHub;


[ServiceFilter(typeof(AuthFilter))]
public class GameHub : Hub
{
    public static readonly string URL = "/game/gameHub";

    public async Task AddClient(Guid gameId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());
    }

    public async Task DeleteClient(Guid gameId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId.ToString());
    }
}
