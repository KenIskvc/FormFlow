namespace FormFlow.MobileApp.DTOs;

public class AnalysisResponseDto
{
    public DateTime CreatedAt { get; set; }
    public int? AnalysisId { get; set; }
    public bool IsPersisted => AnalysisId.HasValue;
    public int ErrorCount { get; set; }
    public string Report { get; set; } = string.Empty;
}
