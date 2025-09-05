namespace TicTacToe.Client.Services.TokenStorageService;

public interface ITokenStorageService
{
    Task<string?> GetAccessTokenAsync();
    Task<string?> GetRefreshTokenAsync();
    Task SetTokensAsync(string accessToken, string refreshToken);
    Task ClearTokensAsync();
    Task<bool> IsContainAccessTokenAsync();
    Task<bool> IsContainRefreshTokenAsync();
}
