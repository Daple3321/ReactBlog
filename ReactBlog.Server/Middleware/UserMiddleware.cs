using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using ReactBlog.Server.Data.Models;
using ReactBlog.Server.Services;

namespace ReactBlog.Server.Middleware;

public class UserMiddleware(ILogger<UserMiddleware> logger, IUserService userService) : IMiddleware
{
    // public async Task InvokeAsync(HttpContext context)
    // {
    // }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var OwnerId = context.User.FindFirstValue("sub");
        if (string.IsNullOrEmpty(OwnerId))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "No Bearer token specified in request headers.",
                Detail = null,
                Type = $"https://httpstatuses.com/{StatusCodes.Status400BadRequest}"
            });
            
            return;
        }

        var newUser = new User()
        {
            Id = OwnerId,
            Username = context.User.FindFirstValue("preferred_username"),
            CreatedAt = DateTime.Now,
            DisplayName = context.User.FindFirstValue("name"),
            Email = context.User.FindFirstValue("email"),
        };
        var createdUser = await userService.CreateUser(newUser);
        if (createdUser == null)
        {
            logger.LogInformation("No user created for id: {userId}. Probably already exists", OwnerId);
        }
        else
        {
            logger.LogInformation("NEW User created with id = {userId}! With name = {name}", OwnerId, context.User.FindFirstValue("preferred_username"));
        }
        

        await next.Invoke(context);
    }
}