namespace FormFlow.MobileApp.DTOs;

public sealed record LoginResponse(
string tokenType,
string accessToken,
int expiresIn,
string refreshToken
);
