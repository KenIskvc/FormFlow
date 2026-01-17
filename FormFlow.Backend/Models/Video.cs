namespace FormFlow.Backend.Models;

public class Video {
    public int Id { get; private set; }
    public required string FileName { get; set; }
    public required byte[] FileData { get; set; }
    public string UserId { get; set; } = default!;
    public virtual FormFlowUser User { get; set; } = default!;
    public virtual ICollection<Analysis> Analyses { get; set; } = [];
}
