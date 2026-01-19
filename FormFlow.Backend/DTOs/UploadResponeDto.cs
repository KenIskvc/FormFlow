namespace FormFlow.Backend.DTOs;

public class UploadResponseDto
{
    public int? VideoId { get; set; }
    public AnalysisDto Analysis { get; set; } = default!;
}

public class AnalysisDto
{
    public DateTime CreatedAt { get; set; }
    public string Report { get; set; } = string.Empty;
}
