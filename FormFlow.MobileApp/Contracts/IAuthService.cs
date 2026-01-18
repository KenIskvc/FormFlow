namespace FormFlow.MobileApp.Contracts;

public interface IAuthService {
    Task<bool> RegisterAsync(string email, string password, CancellationToken ct = default);
    Task<bool> LoginAsync(string email, string password, bool rememberMe, CancellationToken ct = default);
}
