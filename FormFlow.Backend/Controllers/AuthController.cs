using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormFlow.Backend.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AuthController : ControllerBase {

    [HttpGet("me")]
    public IActionResult GetUsername() => Ok(User.Identity?.Name);
}
