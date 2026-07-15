using Microsoft.EntityFrameworkCore;
using ReactBlog.Server.Data.Models;

namespace ReactBlog.Server.Data;

public class BlogContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; } = null!;
    public DbSet<User> Users { get; set; }
    public DbSet<Follow> Follows { get; set; }
    
    public BlogContext(DbContextOptions<BlogContext> options) : base(options) 
    { 
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Follow>(entity =>
        {
            entity.HasKey(f => new { f.FollowerId, f.FollowingId });
                
            // Configure foreign key relationships
            entity
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(f => f.FollowingId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
    }
}
