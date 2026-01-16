using FormFlow.MobileApp.Contracts;
using System.Net.Http.Headers;

namespace FormFlow.MobileApp.Middlewear;

class TokenHandler : DelegatingHandler {
    private readonly ITokenStore _tokenStore;

    public TokenHandler(ITokenStore tokenStore) => _tokenStore = tokenStore;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct) {
        var accessToken = await _tokenStore.GetAccessTokenAsync();
        if (!string.IsNullOrWhiteSpace(accessToken)) {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        return await base.SendAsync(request, ct);
    }
}
