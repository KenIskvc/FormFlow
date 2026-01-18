using FormFlow.Backend.Data;
using FormFlow.Backend.DTOs;
using FormFlow.Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FormFlow.Backend.Controllers;
[Route("api/[controller]")]
[ApiController]
public class VideoController : ControllerBase {

    private readonly FormFlowDbContext _context;
    private readonly UserManager<FormFlowUser> _userManager;

    public VideoController(FormFlowDbContext context, UserManager<FormFlowUser> userManager) {
        _context = context;
        _userManager = userManager;
    }

    // To-Do: Check if user id or name, is it a query param or body form? 
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Get(CancellationToken ct) {
        var userName = User.Identity.Name;
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null) {
            return NotFound("User not found");
        }
        var id = user.Id;
        //this all should be made from a service or repository
        //and is an example of how to fetch data from the database
        //var videos = await _context.Videos.Where(x => x.UserId == id).ToListAsync();
        //return Ok(videos);
        return Ok();
    }

    // To-Do: Read and process video data (uses AI library) and upload if userName is present (binary data []) 
    [HttpPost("upload-video")]
    public IActionResult Upload([FromBody] UploadRequest request, CancellationToken ct) => Ok();

}
