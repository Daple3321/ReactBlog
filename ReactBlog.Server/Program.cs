using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ReactBlog.Server.Data;
using ReactBlog.Server.Services;

namespace ReactBlog.Server;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var authentication = builder.Configuration.GetRequiredSection("Authentication");

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = authentication["Authority"];
                options.Audience = authentication["Audience"];
                options.RequireHttpsMetadata = authentication.GetValue("RequireHttpsMetadata", true);
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    NameClaimType = "preferred_username"
                };
            });

        builder.Services.AddAuthorization();

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddScoped<IBlogService, BlogService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddSqlite<BlogContext>("Data Source=Blogs.db");

        var app = builder.Build();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseDefaultFiles();
        app.UseStaticFiles();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.MapControllers();

        app.MapFallbackToFile("/index.html");

        app.Run();
    }
}
