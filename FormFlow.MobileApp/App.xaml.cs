namespace FormFlow.MobileApp;

public partial class App : Application {
    [Obsolete]
    public App(AppShell appShell) {
        InitializeComponent();
        MainPage = appShell;
    }

    //protected override Window CreateWindow(IActivationState? activationState)
    //{
    //    return new Window(new AppShell());
    //}
}