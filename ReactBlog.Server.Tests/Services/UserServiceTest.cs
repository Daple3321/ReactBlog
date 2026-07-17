using JetBrains.Annotations;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using ReactBlog.Server.Data;
using ReactBlog.Server.Data.Models;
using ReactBlog.Server.Services;
using Xunit;

namespace ReactBlog.Server.Tests.Services;

[TestSubject(typeof(UserService))]
public class UserServiceTest
{
    [Fact]
    public async Task FollowAndUnfollow_UpdatesBothUserLists()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        await using var db = new BlogContext(
            new DbContextOptionsBuilder<BlogContext>()
                .UseSqlite(connection, contextOwnsConnection: true)
                .Options);

        db.Users.AddRange(
            new User { Id = "alice-id", Username = "alice", CreatedAt = DateTime.UtcNow },
            new User { Id = "bob-id", Username = "bob", CreatedAt = DateTime.UtcNow });
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var service = new UserService(NullLogger<UserService>.Instance, db);

        Assert.Equal(FollowResult.Success, await service.FollowUser("alice-id", "bob"));
        Assert.Equal(FollowResult.Success, await service.FollowUser("alice-id", "bob"));
        Assert.Equal("alice", Assert.Single(await service.GetFollowers("bob")).Username);
        Assert.Equal("bob", Assert.Single(await service.GetFollowing("alice")).Username);

        Assert.Equal(FollowResult.Success, await service.UnfollowUser("alice-id", "bob"));
        Assert.Empty(await service.GetFollowers("bob"));
        Assert.Empty(await service.GetFollowing("alice"));
    }
}
