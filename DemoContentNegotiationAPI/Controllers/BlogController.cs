using DemoContentNegotiationAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace DemoContentNegotiationAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BlogController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var blogs = new List<Blog>();
            var blogPosts = new List<BlogPost>
            {
                new BlogPost
                {
                    Title = "Content Negotiation in .NET",
                    MetaDescription = "Content Negotiation For .NET Core",
                    Published = true,
                },
                new BlogPost
                {
                    Title = "Content Negotiation In Various Headers",
                    MetaDescription = "Content Negotiation In CSV And XML",
                    Published = false
                },
            };

            blogs.Add(new Blog
            {
                Name = "Content Negotiation",
                Description = "Demo Content Negotation",
                BlogPosts = blogPosts,
            });
            blogs.Add(new Blog
            {
                Name = "Another one?",
                Description = "Another Blog?",
                BlogPosts = blogPosts
            });

            return Ok(blogs);
        }
    }
}
