using System.ComponentModel.DataAnnotations;

namespace ReactBlog.Server.Data.Models;

public class Blog
{
    [Key]
    public int Id { get; set; }

    public required string Name { get; set; }

    public string? Content { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
}
