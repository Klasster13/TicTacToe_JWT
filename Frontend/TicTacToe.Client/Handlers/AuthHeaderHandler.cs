using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace TicTacToe.Client.Handlers;

public class AuthHeaderHandler(ILocalStorageService localStorage) : DelegatingHandler
{
    private readonly ILocalStorageService _localStorage = localStorage;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _localStorage.GetItemAsync<string>("authToken", cancellationToken);

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}