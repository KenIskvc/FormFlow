using FormFlow.MobileApp.Contracts;
using FormFlow.MobileApp.DTOs;
using System.Diagnostics;

namespace FormFlow.MobileApp.Services;

class TokenStore : ITokenStore {

    private const string AccessKey = "access_token";
    private const string RefreshKey = "refresh_token";
    private const string TokenTypeKey = "token_type";
    private const string ExpiresAtKey = "access_expires_at_utc";

    public async Task SaveAsync(LoginResponse response) {
        await SecureStorage.SetAsync(AccessKey, response.accessToken);
        await SecureStorage.SetAsync(RefreshKey, response.refreshToken);
        await SecureStorage.SetAsync(TokenTypeKey, response.tokenType);

        var expiresAtUtc = DateTime.UtcNow.AddSeconds(response.expiresIn);
        await SecureStorage.SetAsync(ExpiresAtKey, expiresAtUtc.ToString("O"));
    }

    public Task<string?> GetAccessTokenAsync() => SecureStorage.GetAsync(AccessKey);
    public Task<string?> GetRefreshTokenAsync() => SecureStorage.GetAsync(RefreshKey);

    public Task ClearAsync() {
        SecureStorage.Remove(AccessKey);
        SecureStorage.Remove(RefreshKey);
        SecureStorage.Remove(TokenTypeKey);
        SecureStorage.Remove(ExpiresAtKey);
        return Task.CompletedTask;
    }

    public async Task DumpAsync() {
#if DEBUG
        var access = await SecureStorage.GetAsync("access_token");
        var refresh = await SecureStorage.GetAsync("refresh_token");

        Debug.WriteLine("=== TOKEN STORE ===");
        Debug.WriteLine(access is null ? "AccessToken: <null>" : "AccessToken: [present]");
        Debug.WriteLine(refresh is null ? "RefreshToken: <null>" : "RefreshToken: [present]");
#endif
    }
}
