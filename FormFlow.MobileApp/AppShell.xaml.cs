using FormFlow.MobileApp.Contracts;

namespace FormFlow.MobileApp;

public partial class AppShell : Shell {

    private readonly IAuthService _authService;
    public AppShell(IAuthService authService) {
        InitializeComponent();
        _authService = authService;
        Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        UpdateToolbarAsync();
    }

    protected override void OnNavigated(ShellNavigatedEventArgs args) {
        base.OnNavigated(args);
        UpdateToolbarAsync();
    }

    private async void UpdateToolbarAsync() {
        var location = CurrentState.Location.OriginalString;

        // Hide toolbar items on auth pages
        var onAuthPage =
            location.Contains("LoginPage", StringComparison.OrdinalIgnoreCase) ||
            location.Contains(nameof(RegisterPage), StringComparison.OrdinalIgnoreCase);

        if (onAuthPage) {
            RemoveToolbar(LoginToolbarItem);
            RemoveToolbar(LogoutToolbarItem);
            return;
        }

        var isAuth = await _authService.IsAuthenticated();

        if (isAuth) {
            if (ToolbarItems.Contains(LoginToolbarItem))
                ToolbarItems.Remove(LoginToolbarItem);

            if (!ToolbarItems.Contains(LogoutToolbarItem))
                ToolbarItems.Add(LogoutToolbarItem);
        } else {
            if (ToolbarItems.Contains(LogoutToolbarItem))
                ToolbarItems.Remove(LogoutToolbarItem);

            if (!ToolbarItems.Contains(LoginToolbarItem))
                ToolbarItems.Add(LoginToolbarItem);
        }
    }

    private void RemoveToolbar(ToolbarItem item) {
        if (ToolbarItems.Contains(item))
            ToolbarItems.Remove(item);
    }

    private async void OnLoginClicked(object sender, EventArgs e) {
        await Shell.Current.GoToAsync("//LoginPage");
        UpdateToolbarAsync();
    }

    private async void OnLogoutClicked(object sender, EventArgs e) {
        await _authService.LogoutAsync(CancellationToken.None);

        // Send user back to login and hide tabs again
        await Shell.Current.GoToAsync("//LoginPage");
        UpdateToolbarAsync();
    }
}
