using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReactBlog.Server.Services;

namespace ReactBlog.Server.Controllers;

[ApiController]
[Route("users")]
public class UserController(ILogger<UserController> logger, IUserService userService) : ControllerBase
{
    
}