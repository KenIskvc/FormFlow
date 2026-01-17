namespace FormFlow.Backend.DTOs;

public class UploadRequest {
    public required IFormFile VideoFile { get; set; }
    //public byte[] FileData { get; set; }
    //public string FileName { get; set; }
    public string? UserName { get; set; }
}
