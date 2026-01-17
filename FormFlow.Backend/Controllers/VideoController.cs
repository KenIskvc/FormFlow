using FormFlow.Backend.Data;
using FormFlow.Backend.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormFlow.Backend.Controllers;
[Route("api/[controller]")]
[ApiController]
public class VideoController : ControllerBase {

    private readonly FormFlowDbContext _context;

    public VideoController(FormFlowDbContext context) => _context = context;

    // To-Do: Check if user id or name, is it a query param or body form? 
    [HttpGet]
    [Authorize]
    public IActionResult Get(string userId, CancellationToken ct) =>
        //this all should be made from a service or repository
        //and is an example of how to fetch data from the database
        //var videos = await _context.Videos.Where(x => x.UserId == userId).ToListAsync();
        //return Ok(videos);
        Ok();

    // To-Do: Read and process video data (uses AI library) and upload if userName is present (binary data []) 
    [HttpPost("upload-video")]
    public IActionResult Upload([FromBody] UploadRequest request, CancellationToken ct) => Ok();

}
