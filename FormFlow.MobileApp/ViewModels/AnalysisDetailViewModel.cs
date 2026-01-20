using FormFlow.MobileApp.Models;
using FormFlow.MobileApp.Coaching;


namespace FormFlow.MobileApp.ViewModels;

public class AnalysisDetailViewModel
{
    private readonly AnalysisReport _report;

    // Initializes the view model from an AnalysisListItem.
    // The raw JSON report is parsed into an AnalysisReport object.
    // If parsing fails, the detail view cannot function and an exception is thrown.
    public AnalysisDetailViewModel(AnalysisListItem item)
    {
        _report = item.ParsedReport
            ?? throw new InvalidOperationException("Invalid analysis report");
    }

    // Returns a user-friendly summary text showing how many keypoints
    // were detected during the analysis.
    public string DetectedKeypointsText =>
        $"Detected keypoints: {_report.DetectedKeypoints}";

    // Indicates whether the analysis contains any technical errors.
    // Used by the UI to conditionally show or hide the Issues section.
    public bool HasErrors =>
        _report.TechnicalErrors.Any();

    // Exposes the raw technical errors from the analysis report.
    // This represents the "what is wrong" part of the analysis.
    public List<TechnicalError> TechnicalErrors =>
        _report.TechnicalErrors;

    // Provides a UI-friendly representation of all angle measurements.
    // Each angle is transformed into a display object containing:
    // - a normalized, uppercase name
    // - a formatted human-readable range string
    public IEnumerable<object> Angles =>
        _report.Angles.Select(a => new
        {
            Name = a.Key.ToUpperInvariant(),
            Range = FormatAngleRange(a.Value)
        });

    // Formats an angle measurement into a human-readable string.
    private static string FormatAngleRange(AngleReport angle)
    {
        var hasMin = angle.MinDeg.HasValue;
        var hasMax = angle.MaxDeg.HasValue;

        if (hasMin && hasMax)
            return $"{angle.MinDeg:0}° – {angle.MaxDeg:0}°";

        if (hasMax)
            return $"max {angle.MaxDeg:0}°";

        if (hasMin)
            return $"min {angle.MinDeg:0}°";

        return "n/a";
    }

    // Combines technical errors with coaching knowledge.
    // Each error is enriched with a human-readable improvement suggestion
    // based on its error code using the TechnicalErrorCatalog.
    public IEnumerable<ErrorWithSuggestion> Errors =>
    _report.TechnicalErrors.Select(err =>
    {
        var def = TechnicalErrorCatalog.Get(err.Code);

        return new ErrorWithSuggestion
        {
            Message = err.Message,
            Suggestion = def?.Suggestion
        };
    });

    // Provides a flattened, distinct list of coaching suggestions.
    // This represents the "what should I do to improve" section
    // and is independent of how many errors produced the same suggestion.
    public IEnumerable<string> Suggestions =>
        _report.TechnicalErrors
            .Select(e => TechnicalErrorCatalog.Get(e.Code)?.Suggestion)
            .Where(s => !string.IsNullOrWhiteSpace(s))!
            .Distinct();

}
