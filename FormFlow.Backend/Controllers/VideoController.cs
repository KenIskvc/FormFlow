using FormFlow.Backend.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormFlow.Backend.Controllers;
[Route("api/[controller]")]
[ApiController]
public class VideoController : ControllerBase {

    // To-Do: Check if user id or name, is it a query param or body form? 
    [HttpGet]
    [Authorize]
    public IActionResult Get(string userId, CancellationToken ct) => Ok();

    // To-Do: Read and process video data (uses AI library) and upload if userName is present (binary data []) 
    [HttpPost("upload-video")]
    public IActionResult Upload([FromBody] UploadRequest request, CancellationToken ct) => Ok();

}
