using Microsoft.AspNetCore.Mvc;
using ReactBlog.Server.DTOs;
using ReactBlog.Server.Services;

namespace ReactBlog.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BlogController : ControllerBase
    {
        private readonly BlogService blogService;
        private readonly ILogger<BlogController> _logger;
        public BlogController(BlogService blogService, ILogger<BlogController> logger)
        {
            this.blogService = blogService;
            _logger = logger;
        }

        [HttpGet]
        [Route("/blogs/all")]
        public async Task<IEnumerable<Blog>> GetBlogsAsync()
        {
            _logger.Log(LogLevel.Information, "Getting all blogs");
            return await blogService.GetAllAsync();
        }

        [HttpGet]
        [Route("/blogs/{id}")]
        public async Task<ActionResult<Blog>> GetBlog(int id)
        {
            Blog foundBlog = await blogService.GetBlogAsync(id);
            if(foundBlog == null) {
                _logger.Log(LogLevel.Warning, $"Blog with id {id} not found.");
                return NotFound(); 
            }
            else
            {
                _logger.Log(LogLevel.Information, $"Getting blog with id {foundBlog.Id}", foundBlog.Id);
                return Ok(foundBlog);
            }
        }

        [HttpPost]
        [Route("/blogs/new")]
        public IActionResult AddBlog([FromForm] NewBlogDto blogDto)
        {
            if (blogDto == null) { return BadRequest(); }
            //if(newBlog.LastUpdatedAt == default || newBlog.CreatedAt == default) { return BadRequest("createdAt or lastUpdatedAt dates are not specified"); }
            Blog newBlog = new Blog { Id = 0, Name = blogDto.Name, Content = blogDto.Content, CreatedAt = DateTime.Now, LastUpdatedAt = DateTime.Now };

            blogService.AddBlog(newBlog);
            return CreatedAtAction(nameof(GetBlog), new { id = newBlog.Id }, newBlog);
        }

        [HttpPut]
        [Route("/blogs/{id}")]
        public async Task<IActionResult> UpdateBlog([FromRoute] int id, [FromForm] NewBlogDto updatedBlog)
        {
            if (updatedBlog == null) { return BadRequest(); }
            
            //Blog newBlog = new Blog { Id = 0, Name = blogDto.Name, Content = blogDto.Content, CreatedAt = DateTime.Now, LastUpdatedAt = DateTime.Now };

            Blog finalBlog = await blogService.UpdateBlog(id, updatedBlog);
            return CreatedAtAction(nameof(GetBlog), new { id = finalBlog.Id }, finalBlog);
        }

        [HttpDelete]
        [Route("/blogs/{id}")]
        public async Task<IActionResult> RemoveBlog(int id)
        {
            Blog foundBlog = await blogService.GetBlogAsync(id);
            if (foundBlog == null) { return BadRequest(); }

            blogService.RemoveBlog(foundBlog);
            return Ok();
        }
    }
}
