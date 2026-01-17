using Microsoft.AspNetCore.Mvc;

namespace FormFlow.Backend.Controllers;
[Route("api/[controller]")]
[ApiController]
public class VideoController : ControllerBase {

    [HttpGet]
    public IActionResult Get(string userId, CancellationToken ct) => Ok();

    [HttpPost("upload-video")]
    public IActionResult Upload(IFormFile videoFile, CancellationToken ct) => Ok();

}
