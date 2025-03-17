using Blog.Data;
using Blog.Models;
using Blog.ViewModels;
using Blog.ViewModels.Categories;
using Blog.ViewModels.Posts;
using Blog.ViewModels.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    public class PostController : ControllerBase
    {
        [HttpGet("v1/posts")]
        public async Task<IActionResult> GetAsync(
            [FromServices] AppDataContext context,
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 10)
        {
            var count = await context.Posts.AsNoTracking().CountAsync();
            var posts = await context
                .Posts
                .AsNoTracking()
                .Include(x => x.Category)
                .Include(x => x.Author)
                .Select(x => new ListPostsViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Slug = x.Slug,
                    LastUpdateDate = x.LastUpdateDate,
                    Category = x.Category.Name,
                    Author = $"{x.Author.Name} ({x.Author.Email})"
                })
                .Skip(page * pageSize)
                .Take(pageSize)
                .OrderByDescending(x => x.LastUpdateDate)
                .ToListAsync();

            return Ok(new ResultViewModel<dynamic>(new
            {
                total = count,
                page,
                pageSize,
                posts
            }));
        }

        [HttpGet("v1/posts/{id:int}")]
        public async Task<IActionResult> DetailsAsync(
            [FromServices] AppDataContext context,
            [FromRoute] int id)
        {
            try
            {
                var post = await context
                        .Posts
                        .AsNoTracking()
                        .Include(x => x.Author)
                        .ThenInclude(x => x.Roles)
                        .Include(x => x.Category)
                        .FirstOrDefaultAsync(x => x.Id == id);

                if (post == null)
                    return NotFound(new ResultViewModel<Post>("Content not found"));

                var postViewModel = new PostViewModel
                {
                    Id = post.Id,
                    Title = post.Title,
                    Summary = post.Summary,
                    Slug = post.Slug,
                    LastUpdateDate = post.LastUpdateDate,
                    Author = new AuthorViewModel
                    {
                        Id = post.Author.Id,
                        Name = post.Author.Name,
                        Email = post.Author.Email,
                        Slug = post.Author.Slug
                    },
                    Category = new CategoryViewModel
                    {
                        Id = post.Category.Id,
                        Name = post.Category.Name,
                        Slug = post.Category.Slug
                    }
                };

                return Ok(new ResultViewModel<PostViewModel>(postViewModel));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<Post>("01KE02 - Internal server failure"));
            }
        }

        [HttpGet("v1/posts/category/{category}")]
        public async Task<IActionResult> GetBycategoryAsync(
            [FromServices] AppDataContext context,
            [FromRoute] string category,
            [FromQuery] int page = 0,
            [FromQuery] int pagesize = 5)
        {
            var count = await context.Posts
                .AsNoTracking()
                .Where(x => x.Category.Slug == category)
                .CountAsync();

            var posts = await context
                .Posts
                .AsNoTracking()
                .Include(x => x.Author)
                .Include(x => x.Category)
                .Where(x => x.Category.Slug == category)
                .Select(x => new ListPostsViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Slug = x.Slug,
                    LastUpdateDate = x.LastUpdateDate,
                    Category = x.Category.Name,
                    Author = $"{x.Author.Name}"
                })
                .Skip(page * pagesize)
                .Take(pagesize)
                .OrderByDescending(x => x.LastUpdateDate)
                .ToListAsync();

            return Ok(new ResultViewModel<dynamic>(new
            {
                total = count,
                page,
                pagesize,
                posts
            }));
        }
    }
}
