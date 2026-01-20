using FormFlow.MobileApp.Contracts;

namespace FormFlow.MobileApp;

public partial class RegisterPage : ContentPage {

    private readonly IAuthService _authService;
    public RegisterPage(IAuthService authService) {
        InitializeComponent();
        _authService = authService;
    }

    private async void OnRegisterClicked(object? sender, EventArgs e) {
        var email = EmailEntry.Text?.Trim() ?? string.Empty;
        var password = PasswordEntry.Text ?? string.Empty;
        var confirm = ConfirmPasswordEntry.Text ?? string.Empty;

        // Validation as popups
        if (string.IsNullOrWhiteSpace(email)) {
            await DisplayAlert("Validation Error", "Email is required.", "OK");
            return;
        }

        if (password.Length < 6) {
            await DisplayAlert("Validation Error", "Password must be at least 6 characters.", "OK");
            return;
        }

        if (password != confirm) {
            await DisplayAlert("Validation Error", "Passwords do not match.", "OK");
            return;
        }

        SetBusy(true);

        try {
            var success = await _authService.RegisterAsync(email, password);

            if (!success) {
                await DisplayAlert("Error", "Registration failed.", "OK");
                return;
            }

            await DisplayAlert("Success", "Account created.", "OK");
            await Shell.Current.GoToAsync("..");
        } finally {
            SetBusy(false);
        }
    }

    private async void BackToLoginClicked(object? sender, EventArgs e) => await Shell.Current.GoToAsync("..");

    private void SetBusy(bool isBusy) {
        RegisterButton.IsEnabled = !isBusy;
        BusyIndicator.IsVisible = isBusy;
        BusyIndicator.IsRunning = isBusy;
    }
}

