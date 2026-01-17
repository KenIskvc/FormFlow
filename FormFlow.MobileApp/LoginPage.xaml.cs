using FormFlow.MobileApp.Contracts;

namespace FormFlow.MobileApp;

public partial class LoginPage : ContentPage {
    private readonly IAuthService _authService;
    private readonly ITokenStore _tokenStore;

    public LoginPage(IAuthService authService, ITokenStore tokenStore) {
        InitializeComponent();
        _authService = authService;
        _tokenStore = tokenStore;
    }

    private async void OnLoginClicked(object? sender, EventArgs e) {

        var email = EmailEntry.Text?.Trim() ?? string.Empty;
        var password = PasswordEntry.Text ?? string.Empty;
        var rememberMe = RememberMeCheckBox.IsChecked;

        if (string.IsNullOrWhiteSpace(email)) {
            await DisplayAlert("Validation Error", "Email is required.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(password)) {
            await DisplayAlert("Validation Error", "Password is required.", "OK");
            return;
        }

        SetBusy(true);

        try {
            var success = await _authService.LoginAsync(email, password, rememberMe);

            if (!success) {
                await DisplayAlert("Error", "Invalid email or password.", "OK");
                return;
            }

            await DisplayAlert("Success", "You are logged in.", "OK");
            await Shell.Current.GoToAsync("//App/MainPage");

        } finally {
            SetBusy(false);
        }
    }

    private async void OnRegisterClicked(object? sender, EventArgs e) => await Shell.Current.GoToAsync(nameof(RegisterPage));

    private async void OnContinueAsGuestClicked(object? sender, EventArgs e) {
        await _tokenStore.ClearAsync();
        await Shell.Current.GoToAsync("//App/MainPage");
    }
    private void SetBusy(bool isBusy) {
        LoginButton.IsEnabled = !isBusy;
        BusyIndicator.IsVisible = isBusy;
        BusyIndicator.IsRunning = isBusy;
    }
}