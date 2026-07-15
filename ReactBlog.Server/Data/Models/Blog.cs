using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ReactBlog.Server.Data.Models;

public class Blog
{
    [Key]
    public int Id { get; set; }

    [JsonIgnore]
    public string OwnerId { get; set; } = null!;

    public required string Name { get; set; }

    public string? Content { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
}
