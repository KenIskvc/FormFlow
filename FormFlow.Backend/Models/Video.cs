namespace FormFlow.Backend.Models;

public class Video {
    public int Id { get; private set; }
    public required string FileName { get; set; }
    public required byte[] FileData { get; set; }
    public required string? FormFlowUserId { get; set; }
    public ICollection<Analysis> Analyses { get; set; } = [];
}
