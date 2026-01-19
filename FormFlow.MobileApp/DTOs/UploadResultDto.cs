namespace FormFlow.MobileApp.DTOs;

public class UploadResultDto
{
    public int? VideoId { get; set; }
    public AnalysisDto Analysis { get; set; } = default!;
}

public class AnalysisDto
{
    public DateTime CreatedAt { get; set; }
    public string Report { get; set; } = string.Empty;
}
public class VideoListDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
}