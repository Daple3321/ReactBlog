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
    public async Task<ActionResult<User>> GetUserBlogs(string userName)
    {
        var blogs = await userService.GetUserBlogs(userName);
        if (blogs != null) return Ok(blogs);
        
        return NotFound($"User with name {userName} not found");
    }
}