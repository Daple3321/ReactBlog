using Microsoft.EntityFrameworkCore;
using ReactBlog.Server.Data;
using ReactBlog.Server.Data.DTOs;
using ReactBlog.Server.Data.Models;

namespace ReactBlog.Server.Services;

public interface IBlogService
{
    Task<IEnumerable<Blog>> GetAllAsync();
    Task<Blog?> GetBlogAsync(int id);

    Task<Blog> AddBlog(Blog newBlog);

    Task<bool> RemoveBlog(int blogId);

    Task<Blog?> UpdateBlog(int id, NewBlogDto newBlog);
}

public class BlogService(BlogContext blogContext) : IBlogService
{
    public async Task<IEnumerable<Blog>> GetAllAsync()
    {
        return await blogContext.Blogs
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Blog?> GetBlogAsync(int id)
    {
        if (id < 0) return null;
        
        return await blogContext.Blogs
            .FindAsync(id);
    }

    public async Task<Blog> AddBlog(Blog newBlog)
    {
        blogContext.Blogs.Add(newBlog);
        
        await blogContext.SaveChangesAsync();

        return newBlog;
    }

    public async Task<bool> RemoveBlog(int blogId)
    {
        var blog = await GetBlogAsync(blogId);
        if (blog == null) return false;
        
        blogContext.Blogs.Remove(blog);
        
        await blogContext.SaveChangesAsync();

        return true;
    }

    public async Task<Blog?> UpdateBlog(int id, NewBlogDto newBlog)
    {
        var blogToUpdate = await GetBlogAsync(id);
        if (blogToUpdate == null) return null;
        
        blogToUpdate.LastUpdatedAt = DateTime.Now;
        blogToUpdate.Name = newBlog.Name;
        blogToUpdate.Content = newBlog.Content;
        
        await blogContext.SaveChangesAsync();

        return blogToUpdate;
    }
}
