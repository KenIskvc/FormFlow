namespace FormFlow.Backend.DTOs;

public class UploadRequest {
    public required IFormFile File { get; set; } = default!;
    //public byte[] FileData { get; set; }
    //public string FileName { get; set; }
}
