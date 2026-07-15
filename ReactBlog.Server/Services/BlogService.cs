using Microsoft.EntityFrameworkCore;
using ReactBlog.Server.Data;
using ReactBlog.Server.Data.DTOs;
using ReactBlog.Server.Data.Models;

namespace ReactBlog.Server.Services;

public interface IBlogService
{
    Task<IEnumerable<Blog>> GetAllAsync(string ownerId);
    Task<Blog?> GetBlogAsync(int id, string ownerId);

    Task<Blog> AddBlog(Blog newBlog);

    Task<bool> RemoveBlog(int blogId, string ownerId);

    Task<Blog?> UpdateBlog(int id, NewBlogDto newBlog, string ownerId);
}

public class BlogService(BlogContext blogContext) : IBlogService
{
    public async Task<IEnumerable<Blog>> GetAllAsync(string ownerId)
    {
        return await blogContext.Blogs
            .AsNoTracking()
            .Where(blog => blog.OwnerId == ownerId)
            .ToListAsync();
    }

    public async Task<Blog?> GetBlogAsync(int id, string ownerId)
    {
        if (id < 0) return null;
        
        return await blogContext.Blogs
            .FirstOrDefaultAsync(blog => blog.Id == id && blog.OwnerId == ownerId);
    }

    public async Task<Blog> AddBlog(Blog newBlog)
    {
        blogContext.Blogs.Add(newBlog);
        
        await blogContext.SaveChangesAsync();

        return newBlog;
    }

    public async Task<bool> RemoveBlog(int blogId, string ownerId)
    {
        var blog = await GetBlogAsync(blogId, ownerId);
        if (blog == null) return false;
        
        blogContext.Blogs.Remove(blog);
        
        await blogContext.SaveChangesAsync();

        return true;
    }

    public async Task<Blog?> UpdateBlog(int id, NewBlogDto newBlog, string ownerId)
    {
        var blogToUpdate = await GetBlogAsync(id, ownerId);
        if (blogToUpdate == null) return null;
        
        blogToUpdate.LastUpdatedAt = DateTime.Now;
        blogToUpdate.Name = newBlog.Name;
        blogToUpdate.Content = newBlog.Content;
        
        await blogContext.SaveChangesAsync();

        return blogToUpdate;
    }
}
