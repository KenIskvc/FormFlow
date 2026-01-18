namespace FormFlow.Backend.DTOs;

public class UploadRequest {
    public required IFormFile File { get; set; } = default!;
    
}
