namespace ReactBlog.Server.Data.Models;

public class User
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<Blog>? Blogs { get; set; } = null;

    public List<Follow> Following { get; set; } = [];
    public List<Follow> Followers { get; set; } = [];
}