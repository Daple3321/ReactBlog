namespace ReactBlog.Server.Data.DTOs;

public class NewBlogDto
{
    public required string Name { get; set; }
    public string? Content { get; set; }
}
