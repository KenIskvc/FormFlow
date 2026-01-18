using System.Text;

namespace FormFlow.MobileApp;
public partial class MainPage : ContentPage {
    private readonly HttpClient _client;
    public MainPage(HttpClient client) {
        InitializeComponent();
        _client = client;
    }

    // private void OnCounterClicked(object? sender, EventArgs e)
    // {
    //     count++;
    //
    //     if (count == 1)
    //         CounterBtn.Text = $"Clicked {count} time";
    //     else
    //         CounterBtn.Text = $"Clicked {count} times";
    //
    //     SemanticScreenReader.Announce(CounterBtn.Text);
    // }

    private void NavigateToUpload(object? sender, EventArgs e) {

        /*if(count == 1)
            CounterBtn.Text = $"Clicked {count} time";
        else
            CounterBtn.Text = $"Clicked {count} times";

        SemanticScreenReader.Announce(CounterBtn.Text);*/
    }

    protected override async void OnAppearing() {
        base.OnAppearing();
        await RefreshTokenInfoAsync();
    }

    private async void OnRefreshTokenInfoClicked(object? sender, EventArgs e) => await RefreshTokenInfoAsync();

    private async Task RefreshTokenInfoAsync() {
        // Match the keys you used in your TokenStore
        var access = await SecureStorage.GetAsync("access_token");
        var refresh = await SecureStorage.GetAsync("refresh_token");
        var tokenType = await SecureStorage.GetAsync("token_type");
        var expiresAtRaw = await SecureStorage.GetAsync("access_expires_at_utc");

        var sb = new StringBuilder();

        sb.AppendLine($"Access token: {(string.IsNullOrWhiteSpace(access) ? "NOT SET" : "PRESENT")}");
        sb.AppendLine($"Refresh token: {(string.IsNullOrWhiteSpace(refresh) ? "NOT SET" : "PRESENT")}");
        sb.AppendLine($"Token type: {(!string.IsNullOrWhiteSpace(tokenType) ? tokenType : "(not stored)")}");

        if (DateTime.TryParse(expiresAtRaw, null, System.Globalization.DateTimeStyles.RoundtripKind, out var expiresAtUtc)) {
            var remaining = expiresAtUtc - DateTime.UtcNow;
            sb.AppendLine($"Expires at (UTC): {expiresAtUtc:O}");
            sb.AppendLine($"Expires in: {(remaining.TotalSeconds <= 0 ? "EXPIRED" : $"{(int)remaining.TotalMinutes} min")}");
        } else {
            sb.AppendLine("Expires at (UTC): (not stored)");
        }

        TokenInfoEditor.Text = sb.ToString();
    }

    private async void OnTestAuthMeClicked(object? sender, EventArgs e) {
        try {
            AuthTestResultLabel.Text = "Calling /auth/me ...";

            var resp = await _client.GetAsync("/auth/me");

            if (!resp.IsSuccessStatusCode) {
                AuthTestResultLabel.Text =
                    $"ERROR: {(int)resp.StatusCode} {resp.ReasonPhrase}";
                return;
            }

            var username = await resp.Content.ReadAsStringAsync();
            AuthTestResultLabel.Text = $"Backend says user = {username}";
        } catch (Exception ex) {
            AuthTestResultLabel.Text = $"Exception: {ex.Message}";
        }
    }
}
