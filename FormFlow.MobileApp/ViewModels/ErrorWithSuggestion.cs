namespace FormFlow.MobileApp.ViewModels;

public class ErrorWithSuggestion
{
    // Contains the human-readable description of the technical issue.
    public string Message { get; init; } = string.Empty;

    // Contains an optional coaching suggestion associated with the error.
    public string? Suggestion { get; init; }

    // Indicates whether a coaching suggestion is available.
    public bool HasSuggestion =>
        !string.IsNullOrWhiteSpace(Suggestion);
}
