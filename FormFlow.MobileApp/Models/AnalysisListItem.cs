public class AnalysisListItem
{
    public int? AnalysisId { get; init; }
    public DateTime CreatedAt { get; init; }
    public int ErrorCount { get; init; }
    public bool IsPersisted => AnalysisId.HasValue;
    public string Report { get; init; } = string.Empty;
    public string VideoTitle { get; init; } = string.Empty;


    // ---------- UI-only ----------
    public string Title => VideoTitle;

    public string Date =>
        CreatedAt.ToString("dd.MM.yyyy HH:mm");

    public string Status =>
        ErrorCount == 0 ? "OK" : "Issues detected";

    public Color StatusColor =>
        ErrorCount == 0 ? Colors.Green : Colors.Red;

    public string ErrorCountText =>
        ErrorCount == 0 ? "No errors" : $"{ErrorCount} error(s)";
}
