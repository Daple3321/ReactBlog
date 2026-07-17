using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReactBlog.Server.Data.Models;
using ReactBlog.Server.Services;

namespace ReactBlog.Server.Controllers;

[ApiController]
[Route("users")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet]
    public async Task<PagedResult<User>> GetUsersAsync()
    {
        return await userService.GetUsers();
    }

    [HttpGet("{userName}")]
    public async Task<ActionResult<User>> GetUser(string userName)
    {
        var foundUser = await userService.GetUserByName(userName);
        if (foundUser != null) return Ok(foundUser);
        
        return NotFound($"User with name {userName} not found");
    }
    
    [HttpGet("{userName}/blogs")]
    public async Task<ActionResult<PagedResult<Blog>>> GetUserBlogs(string userName)
    {
        var blogs = await userService.GetUserBlogs(userName);
        if (blogs != null) return Ok(blogs);
        
        return NotFound($"User with name {userName} not found");
    }

    [HttpGet("{userName}/followers")]
    public async Task<ActionResult<List<UserSummary>>> GetFollowers(string userName)
    {
        if (await userService.GetUserByName(userName) == null)
            return NotFound($"User with name {userName} not found");

        return Ok(await userService.GetFollowers(userName));
    }

    [HttpGet("{userName}/following")]
    public async Task<ActionResult<List<UserSummary>>> GetFollowing(string userName)
    {
        if (await userService.GetUserByName(userName) == null)
            return NotFound($"User with name {userName} not found");

        return Ok(await userService.GetFollowing(userName));
    }

    [Authorize]
    [HttpPost("{userName}/follow")]
    public async Task<IActionResult> Follow(string userName)
    {
        var userId = User.FindFirstValue("sub");
        if (userId == null) return Unauthorized();

        return await userService.FollowUser(userId, userName) switch
        {
            FollowResult.Success => NoContent(),
            FollowResult.TargetNotFound => NotFound($"User with name {userName} not found"),
            FollowResult.CannotFollowSelf => BadRequest("You cannot follow yourself."),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [Authorize]
    [HttpDelete("{userName}/follow")]
    public async Task<IActionResult> Unfollow(string userName)
    {
        var userId = User.FindFirstValue("sub");
        if (userId == null) return Unauthorized();

        return await userService.UnfollowUser(userId, userName) switch
        {
            FollowResult.Success => NoContent(),
            FollowResult.TargetNotFound => NotFound($"User with name {userName} not found"),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}