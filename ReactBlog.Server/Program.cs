using ReactBlog.Server.Data;
using ReactBlog.Server.Services;

namespace ReactBlog.Server;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddScoped<IBlogService, BlogService>();
        builder.Services.AddSqlite<BlogContext>("Data Source=Blogs.db");
        builder.Services.AddDbContext<BlogContext>();

        var app = builder.Build();

        app.UseDefaultFiles();
        app.UseStaticFiles();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        //app.UseAuthorization();

        app.MapControllers();

        app.MapFallbackToFile("/index.html");

        app.Run();
    }
}
