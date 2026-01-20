using FormFlow.MobileApp.Contracts;
using FormFlow.MobileApp.DTOs;
using System.Net.Http.Json;

namespace FormFlow.MobileApp.Services;

class AuthService : IAuthService {
    readonly HttpClient _client;
    private readonly ITokenStore _tokenStore;

    public AuthService(HttpClient client, ITokenStore tokenStore) {
        _client = client;
        _tokenStore = tokenStore;
    }

    public async Task<bool> RegisterAsync(string email, string password, CancellationToken ct = default) {
        var resp = await _client.PostAsJsonAsync("/register", new { email, password }, ct);

        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> LoginAsync(string email, string password, bool rememberMe, CancellationToken ct) {
        var resp = await _client.PostAsJsonAsync(
            "/login",
            new { email, password, rememberMe },
            ct
        );

        if (!resp.IsSuccessStatusCode)
            return false;

        var tokens = await resp.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken: ct);
        if (tokens is null || string.IsNullOrWhiteSpace(tokens.accessToken))
            return false;

        await _tokenStore.SaveAsync(tokens);
        return true;
    }
    public Task LogoutAsync(CancellationToken ct) => _tokenStore.ClearAsync();

    public async Task<bool> IsAuthenticated() {
        var token = await _tokenStore.GetAccessTokenAsync();
        return token != null;
    }

}
