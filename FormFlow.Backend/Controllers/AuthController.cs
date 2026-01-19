using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormFlow.Backend.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AuthController : ControllerBase {

    /// <summary>
    /// Returns the username of the currently authenticated user.
    /// </summary>
    /// <returns>
    /// An <see cref="IActionResult"/> containing the username from the current
    /// <see cref="System.Security.Claims.ClaimsPrincipal"/> if the user is authenticated;
    /// otherwise returns <c>null</c>.
    /// </returns>
    [HttpGet("me")]
    public IActionResult GetUsername() => Ok(User.Identity?.Name);
}
