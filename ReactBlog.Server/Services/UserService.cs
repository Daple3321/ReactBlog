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
    Task<List<UserSummary>> GetFollowers(string userName);
    Task<List<UserSummary>> GetFollowing(string userName);
    Task<FollowResult> FollowUser(string followerId, string userName);
    Task<FollowResult> UnfollowUser(string followerId, string userName);

    Task<User> EnsureUser(User newUser);
}

public record UserSummary(string Id, string Username);

public enum FollowResult
{
    Success,
    TargetNotFound,
    CannotFollowSelf
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

    public async Task<List<UserSummary>> GetFollowers(string userName)
    {
        var userId = await GetUserId(userName);
        if (userId == null) return [];

        return await context.Follows
            .Where(follow => follow.FollowingId == userId)
            .Join(
                context.Users,
                follow => follow.FollowerId,
                user => user.Id,
                (_, user) => user)
            .OrderBy(user => user.Username)
            .Select(user => new UserSummary(user.Id, user.Username))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<UserSummary>> GetFollowing(string userName)
    {
        var userId = await GetUserId(userName);
        if (userId == null) return [];

        return await context.Follows
            .Where(follow => follow.FollowerId == userId)
            .Join(
                context.Users,
                follow => follow.FollowingId,
                user => user.Id,
                (_, user) => user)
            .OrderBy(user => user.Username)
            .Select(user => new UserSummary(user.Id, user.Username))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<FollowResult> FollowUser(string followerId, string userName)
    {
        var followingId = await GetUserId(userName);
        if (followingId == null) return FollowResult.TargetNotFound;
        if (followingId == followerId) return FollowResult.CannotFollowSelf;

        var alreadyFollowing = await context.Follows
            .AnyAsync(follow => follow.FollowerId == followerId && follow.FollowingId == followingId);
        if (alreadyFollowing) return FollowResult.Success;

        context.Follows.Add(new Follow(followerId, followingId));
        await context.SaveChangesAsync();
        return FollowResult.Success;
    }

    public async Task<FollowResult> UnfollowUser(string followerId, string userName)
    {
        var followingId = await GetUserId(userName);
        if (followingId == null) return FollowResult.TargetNotFound;

        var follow = await context.Follows.FindAsync(followerId, followingId);
        if (follow != null)
        {
            context.Follows.Remove(follow);
            await context.SaveChangesAsync();
        }

        return FollowResult.Success;
    }

    private async Task<string?> GetUserId(string userName)
    {
        return await context.Users
            .Where(user => user.Username == userName)
            .Select(user => user.Id)
            .FirstOrDefaultAsync();
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
