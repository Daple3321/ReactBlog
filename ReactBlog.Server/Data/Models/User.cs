namespace ReactBlog.Server.Data.Models;

public class User
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // should be <User>?
    public List<Follow>? Follows { get; set; } = new();
}