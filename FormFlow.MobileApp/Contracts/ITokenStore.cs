using FormFlow.MobileApp.DTOs;

namespace FormFlow.MobileApp.Contracts;

public interface ITokenStore {
    Task SaveAsync(LoginResponse tokens);
    Task<string?> GetAccessTokenAsync();
    Task<string?> GetRefreshTokenAsync();
    Task ClearAsync();
    Task DumpAsync();
}
