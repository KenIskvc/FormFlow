namespace FormFlow.Backend.Models;

public class Analysis {
    public int Id { get; private set; }
    public DateTime CreatedAt { get; set; }
    public required string Report { get; set; }
    public int VideoId { get; set; }

}
