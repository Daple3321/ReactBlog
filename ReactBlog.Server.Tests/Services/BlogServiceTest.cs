using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ReactBlog.Server.Data;
using ReactBlog.Server.Data.DTOs;
using ReactBlog.Server.Data.Models;
using ReactBlog.Server.Services;
using Xunit;

namespace ReactBlog.Server.Tests.Services;

[TestSubject(typeof(BlogService))]
public class BlogServiceTest
{
    private static BlogContext CreateDb(string dbName) =>
        new(new DbContextOptionsBuilder<BlogContext>().UseSqlite().Options);

    private static BlogService CreateService(BlogContext db) => new(db);
    
    private static Blog SeedBlog(BlogContext db, string name = "Test Blog", string content = "Test Content")
    {
        var blog = new Blog()
        {
            Name = "TestBlog",
            Content = "Some test content!",
            CreatedAt = DateTime.Now,
            LastUpdatedAt = DateTime.Now
        };
        db.Blogs.Add(blog);
        db.SaveChanges();
        return blog;
    }
    
    [Fact]
    public async Task GetBlogAsync_Item_Missing()
    {
        await using var db = CreateDb(nameof(GetBlogAsync_Item_Missing));
        
        var blog = await CreateService(db).GetBlogAsync(1000);
        
        Assert.Null(blog);
    }
    
    [Fact]
    public async Task GetBlogAsync_ItemId_Negative()
    {
        await using var db = CreateDb(nameof(GetBlogAsync_Item_Missing));
        
        var blog = await CreateService(db).GetBlogAsync(-15);
        
        Assert.Null(blog);
    }
    
    [Fact]
    public async Task GetBlogAsync_ReturnsItem_WhenFound()
    {
        await using var db = CreateDb(nameof(GetBlogAsync_Item_Missing));
        SeedBlog(db);
        
        var blog = await CreateService(db).GetBlogAsync(1);
        
        Assert.NotNull(blog);
    }
    
    [Fact]
    public async Task AddBlog_CreatesBlog()
    {
        await using var db = CreateDb(nameof(GetBlogAsync_Item_Missing));
        var service = CreateService(db);

        var newBlog = await service.AddBlog(new Blog()
        {
            Name = "Test Blog",
            Content = "asdasda",
            CreatedAt = DateTime.Now,
            LastUpdatedAt = DateTime.Now
        });
            
        Assert.NotNull(newBlog);
        Assert.True(newBlog.Id > 0);
    }
    
    [Fact]
    public async Task RemoveBlog_RemovesExistingBlog()
    {
        await using var db = CreateDb(nameof(GetBlogAsync_Item_Missing));
        var service = CreateService(db);
        //SeedBlog(db);
        
        var newBlog = await service.AddBlog(new Blog()
        {
            Name = "Test Blog",
            Content = "asdasda",
            CreatedAt = DateTime.Now,
            LastUpdatedAt = DateTime.Now
        });
        
        var removed = await service.RemoveBlog(newBlog.Id);
        var blog = await service.GetBlogAsync(1);
        
        Assert.True(removed, "Blog wasn't removed. False returned");
        Assert.Null(blog);
    }
    
    [Fact]
    public async Task RemoveBlog_NonExistent()
    {
        await using var db = CreateDb(nameof(GetBlogAsync_Item_Missing));
        var service = CreateService(db);
        
        var removed = await service.RemoveBlog(1);
        
        Assert.False(removed, "Blog was removed. True returned");
    }
    
    [Fact]
    public async Task UpdateBlog_NonExistent()
    {
        await using var db = CreateDb(nameof(GetBlogAsync_Item_Missing));
        var service = CreateService(db);
        
        var updateBlog = await service.UpdateBlog(1, new NewBlogDto()
        {
            Name = "Updated Name",
            Content = "Updated Content"
        });

        Assert.Null(updateBlog);
    }
    
    [Fact]
    public async Task UpdateBlog_FieldsUpdated()
    {
        await using var db = CreateDb(nameof(GetBlogAsync_Item_Missing));
        var service = CreateService(db);

        var creationTime = DateTime.Now;
        var newBlog = await service.AddBlog(new Blog()
        {
            Name = "Test Blog",
            Content = "asdasda",
            CreatedAt = creationTime,
            LastUpdatedAt = creationTime
        });
        
        var updateBlog = await service.UpdateBlog(newBlog.Id, new NewBlogDto()
        {
            Name = "Updated Name",
            Content = "Updated Content"
        });

        Assert.NotNull(updateBlog);
        Assert.True(updateBlog.Name == "Updated Name", "Name field wasn't updated");
        Assert.True(updateBlog.Content == "Updated Content", "Content field wasn't updated");
        Assert.True(updateBlog.LastUpdatedAt != creationTime, "LastUpdateAt field wasn't updated");
    }
}