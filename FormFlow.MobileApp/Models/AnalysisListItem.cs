using System.Text.Json;
using FormFlow.MobileApp.Models;

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

    // Parses the raw JSON report into a strongly typed AnalysisReport object.
    // This allows the rest of the application to work with structured data
    // instead of manually parsing JSON in the UI or ViewModels.
    // Returns null if the report is empty or cannot be deserialized.
    public AnalysisReport? ParsedReport
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Report))
                return null;

            try
            {
                return JsonSerializer.Deserialize<AnalysisReport>(
                    Report,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
            catch
            {
                return null;
            }
        }
    }

}
