using Microsoft.EntityFrameworkCore;
using ReactBlog.Server.Data;
using ReactBlog.Server.Data.Models;

namespace ReactBlog.Server.Services;

public interface IUserService
{
    Task<User?> GetUserById(string userId);
    Task<User?> GetUserByName(string userName);
    Task<PagedResult<User>> GetUsers(int page = 1, int pageSize = 10);
    Task<PagedResult<Blog>?> GetUserBlogs(string userId, int page = 1, int pageSize = 10);

    Task<User> EnsureUser(User newUser);
}

public class UserService(ILogger<UserService> logger, BlogContext context) : IUserService
{
    public async Task<User?> GetUserById(string userId)
    {
        return await context.Users
            .AsNoTracking()
            .Include(x => x.Following)
            .Include(x => x.Followers)
            .Include(x => x.Blogs)
            .FirstOrDefaultAsync(x => x.Id == userId);
    }

    public async Task<User?> GetUserByName(string userName)
    {
        return await context.Users
            .AsNoTracking()
            .Include(x => x.Following)
            .Include(x => x.Followers)
            .Include(x => x.Blogs)
            .FirstOrDefaultAsync(x => x.Username == userName);
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

    public async Task<PagedResult<Blog>?> GetUserBlogs(string userName, int page = 1, int pageSize = 10)
    {
        var user = await GetUserByName(userName);
        if (user == null) return null;

        int itemsToSkip = (page - 1) * pageSize;
        int totalItems = await context.Blogs.CountAsync(x => x.OwnerId == user.Id);

        var blogs = await context.Blogs
            .AsNoTracking()
            .Where(x => x.OwnerId == user.Id)
            .Skip(itemsToSkip)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Blog>(blogs, totalItems, page, pageSize);
    }

    public async Task<User> EnsureUser(User newUser)
    {
        var existing = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == newUser.Id);
        if (existing != null) return existing;

        context.Users.Add(newUser);

        try
        {
            await context.SaveChangesAsync();
            logger.LogInformation("Created user {UserId} ({Username})", newUser.Id, newUser.Username);
            return newUser;
        }
        catch (DbUpdateException)
        {
            context.ChangeTracker.Clear();
            return await context.Users.AsNoTracking().FirstAsync(x => x.Id == newUser.Id);
        }
    }
}
