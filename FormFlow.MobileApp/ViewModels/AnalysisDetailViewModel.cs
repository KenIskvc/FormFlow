using FormFlow.MobileApp.Models;
using FormFlow.MobileApp.Coaching;


namespace FormFlow.MobileApp.ViewModels;

public class AnalysisDetailViewModel
{
    private readonly AnalysisReport _report;

    public AnalysisDetailViewModel(AnalysisListItem item)
    {
        _report = item.ParsedReport
            ?? throw new InvalidOperationException("Invalid analysis report");
    }

    public string DetectedKeypointsText =>
        $"Detected keypoints: {_report.DetectedKeypoints}";

    public bool HasErrors =>
        _report.TechnicalErrors.Any();

    public List<TechnicalError> TechnicalErrors =>
        _report.TechnicalErrors;

    public IEnumerable<object> Angles =>
        _report.Angles.Select(a => new
        {
            Name = a.Key.ToUpperInvariant(),
            Range = FormatAngleRange(a.Value)
        });

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

    public IEnumerable<string> Suggestions =>
        _report.TechnicalErrors
            .Select(e => TechnicalErrorCatalog.Get(e.Code)?.Suggestion)
            .Where(s => !string.IsNullOrWhiteSpace(s))!
            .Distinct();

}
