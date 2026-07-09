using Microsoft.AspNetCore.Mvc;
using ReactBlog.Server.Data.DTOs;
using ReactBlog.Server.Data.Models;
using ReactBlog.Server.Services;

namespace ReactBlog.Server.Controllers;

[ApiController]
[Route("blogs")]
public class BlogController(IBlogService blogService, ILogger<BlogController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<Blog>> GetBlogsAsync()
    {
        return await blogService.GetAllAsync();
    }

    [HttpGet("{blogId:int}")]
    public async Task<ActionResult<Blog>> GetBlog(int blogId)
    {
        var foundBlog = await blogService.GetBlogAsync(blogId);
        if (foundBlog != null) return Ok(foundBlog);
        
        logger.LogInformation("Blog with id {blogId} not found.", blogId);
        return NotFound($"Blog with id {blogId} not found");
    }

    [HttpPost]
    public async Task<ActionResult<Blog>> AddBlog([FromForm] NewBlogDto blogDto)
    {
        if (blogDto == null || string.IsNullOrEmpty(blogDto.Name)) { return BadRequest(); }

        var newBlog = new Blog { Id = 0, Name = blogDto.Name, Content = blogDto.Content, CreatedAt = DateTime.Now, LastUpdatedAt = DateTime.Now };

        await blogService.AddBlog(newBlog);
        
        return CreatedAtAction(nameof(GetBlog), new { blogId = newBlog.Id }, newBlog);
    }

    [HttpPut("{blogId:int}")]
    public async Task<ActionResult<Blog>> UpdateBlog(int blogId, [FromForm] NewBlogDto updatedBlog)
    {
        if (updatedBlog == null) { return BadRequest(); }
        
        var blog = await blogService.UpdateBlog(blogId, updatedBlog);
        
        if (blog == null) return NotFound($"Blog {blogId} not found");
        return Ok(blog);
    }

    [HttpDelete("{blogId:int}")]
    public async Task<IActionResult> RemoveBlog(int blogId)
    {
        var deleted = await blogService.RemoveBlog(blogId);
        if (!deleted) { return BadRequest($"Blog {blogId} not found"); }
        
        return Ok("Blog deleted.");
    }
}
