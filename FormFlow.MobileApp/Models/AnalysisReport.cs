namespace FormFlow.MobileApp.Models;

public class AnalysisReport
{
    public int DetectedKeypoints { get; set; }
    public Dictionary<string, AngleReport> Angles { get; set; } = new();
    public List<TechnicalError> TechnicalErrors { get; set; } = new();
}
