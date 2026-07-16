using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReactBlog.Server.Data.Models;
using ReactBlog.Server.Services;

namespace ReactBlog.Server.Controllers;

[ApiController]
[Authorize]
[Route("me")]
public class MeController(IUserService userService) : ControllerBase
{
    [HttpGet]
    [HttpPost]
    public async Task<ActionResult<User>> EnsureMe()
    {
        var sub = User.FindFirstValue("sub");
        if (string.IsNullOrEmpty(sub))
        {
            return Unauthorized();
        }

        var user = await userService.EnsureUser(new User
        {
            Id = sub,
            Username = User.FindFirstValue("preferred_username") ?? sub,
            DisplayName = User.FindFirstValue("name"),
            Email = User.FindFirstValue("email"),
            CreatedAt = DateTime.UtcNow,
        });

        return Ok(user);
    }
}
