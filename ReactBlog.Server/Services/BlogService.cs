using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReactBlog.Server.DTOs;

namespace ReactBlog.Server.Services
{
    public class BlogService
    {
        private readonly BlogContext _context;

        public BlogService(BlogContext blogContext)
        {
            _context = blogContext;
        }

        public async Task<IEnumerable<Blog>> GetAllAsync()
        {
            return await _context.Blogs
            .AsNoTracking()
            .ToListAsync();
        }

        public async Task<Blog?> GetBlogAsync(int id)
        {
            return await _context.Blogs
            .FindAsync(id);
        }

        public Blog? GetBlog(int id)
        {
            return _context.Blogs.Find(id);
        }

        public void AddBlog(Blog newBlog)
        {
            _context.Blogs.Add(newBlog);
            _context.SaveChanges();
        }

        public void RemoveBlog(Blog blog)
        {
            if (blog != null)
            {
                _context.Blogs.Remove(blog);
                _context.SaveChanges();
            }
        }

        public async Task<Blog?> UpdateBlog(int id, NewBlogDto newBlog)
        {
            Blog blogToUpdate = await GetBlogAsync(id);
            if(blogToUpdate != null)
            {
                blogToUpdate.LastUpdatedAt = DateTime.Now;
                blogToUpdate.Name = newBlog.Name;
                blogToUpdate.Content = newBlog.Content;
            }
            await _context.SaveChangesAsync();

            return blogToUpdate;
        }
    }
}
