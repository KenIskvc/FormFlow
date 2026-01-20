using FormFlow.MobileApp.Contracts;
using FormFlow.MobileApp.Middlewear;
using FormFlow.MobileApp.Services;
using Microsoft.Extensions.Logging;


namespace FormFlow.MobileApp;

public static class MauiProgram {
    public static MauiApp CreateMauiApp() {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<ITokenStore, TokenStore>();

        builder.Services.AddSingleton<AppShell>();

        builder.Services.AddTransient<TokenHandler>();

        builder.Services.AddHttpClient("FormFlow.Backend", client => {
            client.BaseAddress = new Uri("https://localhost:7110");
            client.Timeout = TimeSpan.FromSeconds(30);
        })
        .AddHttpMessageHandler<TokenHandler>();

        builder.Services.AddTransient(sp =>
            sp.GetRequiredService<IHttpClientFactory>().CreateClient("FormFlow.Backend"));

        builder.Services.AddTransient<IAnalysisApi, AnalysisApi>();
        builder.Services.AddTransient<IAuthService, AuthService>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
