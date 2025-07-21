using Microsoft.EntityFrameworkCore;

namespace ReactBlog.Server
{
    public class BlogContext : DbContext
    {
        public BlogContext(DbContextOptions<BlogContext> options) : base(options) 
        { 
            Database.EnsureCreated();
            //if (!Blogs.Any())
            //{
            //    Blogs.Add(new Blog() { Name = "Some blog", Content = "BIG CONENTTEAS", CreatedAt = DateTime.Now, LastUpdatedAt = DateTime.Now });
            //    Blogs.Add(new Blog() { Name = "Another blog", Content = "ASDASDASD", CreatedAt = DateTime.Now, LastUpdatedAt = DateTime.Now });
            //    SaveChanges();
            //}
        }

        public DbSet<Blog> Blogs { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite("Data source=Blogs.db");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
