namespace FormFlow.Backend.DTOs;

public class AnalysisResponseDto
{
    public DateTime CreatedAt { get; set; }
    public int? AnalysisId { get; set; }
    public bool IsPersisted => AnalysisId.HasValue;
    public int ErrorCount { get; set; }
    public string Report { get; set; } = default!;
    public string VideoTitle { get; set; } = string.Empty;
}
