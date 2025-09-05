using Blazored.LocalStorage;

namespace TicTacToe.Client.Services.TokenStorageService.Impl;

public class TokenStorageService(ILocalStorageService localStorage) : ITokenStorageService
{
    private readonly ILocalStorageService _localStorage = localStorage;
    private const string AccessTokenKey = "accessToken";
    private const string RefreshTokenKey = "refreshToken";


    public async Task<string?> GetAccessTokenAsync() => await _localStorage.GetItemAsync<string>(AccessTokenKey);
    public async Task<string?> GetRefreshTokenAsync() => await _localStorage.GetItemAsync<string>(RefreshTokenKey);


    public async Task SetTokensAsync(string accessToken, string refreshToken)
    {
        await _localStorage.SetItemAsync(AccessTokenKey, accessToken);
        await _localStorage.SetItemAsync(RefreshTokenKey, refreshToken);
    }


    public async Task ClearTokensAsync()
    {
        await _localStorage.RemoveItemAsync(AccessTokenKey);
        await _localStorage.RemoveItemAsync(RefreshTokenKey);
    }

    public async Task<bool> IsContainAccessTokenAsync() => await _localStorage.ContainKeyAsync(AccessTokenKey);
    public async Task<bool> IsContainRefreshTokenAsync() => await _localStorage.ContainKeyAsync(RefreshTokenKey);
}
