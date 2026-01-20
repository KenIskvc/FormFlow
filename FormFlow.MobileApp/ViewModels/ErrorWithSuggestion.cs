namespace FormFlow.MobileApp.ViewModels;

public class ErrorWithSuggestion
{
    public string Message { get; init; } = string.Empty;
    public string? Suggestion { get; init; }

    public bool HasSuggestion =>
        !string.IsNullOrWhiteSpace(Suggestion);
}
