using Microsoft.EntityFrameworkCore;
using ReactBlog.Server.Data;
using ReactBlog.Server.Data.Models;

namespace ReactBlog.Server.Services;

public interface IUserService
{
    Task<User?> GetUser(string userId);
    Task<PagedResult<User>> GetUsers(int page = 1, int pageSize = 10);
    Task<PagedResult<Blog>> GetUserBlogs(string userId, int page = 1, int pageSize = 10);

    Task<User?> CreateUser(User newUser);
}

public class UserService(ILogger<UserService> logger, BlogContext context) : IUserService
{
    public async Task<User?> GetUser(string userId)
    {
        var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId);

        return user;
    }

    public async Task<PagedResult<User>> GetUsers(int page = 1, int pageSize = 10)
    {
        int itemsToSkip = (page - 1) * pageSize;
        int totalItems = await context.Users.CountAsync();

        var users = await context.Users
            .Skip(itemsToSkip)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
        
        return new PagedResult<User>(users, totalItems, page, pageSize);
    }

    public async Task<PagedResult<Blog>> GetUserBlogs(string userId, int page = 1, int pageSize = 10)
    {
        int itemsToSkip = (page - 1) * pageSize;
        int totalItems = await context.Users.CountAsync();
        
        var blogs = await context.Blogs
            .AsNoTracking()
            .Skip(itemsToSkip)
            .Take(pageSize)
            .Where(x => x.OwnerId == userId)
            .ToListAsync();

        return new PagedResult<Blog>(blogs, totalItems, page, pageSize);
    }

    public async Task<User?> CreateUser(User newUser)
    {
        var prevUser = await GetUser(newUser.Id);
        if (prevUser != null) return null;

        context.Users.Add(newUser);

        await context.SaveChangesAsync();
        
        return newUser;
    }
}