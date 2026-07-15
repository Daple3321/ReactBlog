using Microsoft.EntityFrameworkCore;
using ReactBlog.Server.Data.Models;

namespace ReactBlog.Server.Data;

public class BlogContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; } = null!;
    
    public BlogContext(DbContextOptions<BlogContext> options) : base(options) 
    { 
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}
