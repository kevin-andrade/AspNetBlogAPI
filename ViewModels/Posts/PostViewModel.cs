using Blog.ViewModels.Categories;
using Blog.ViewModels.Users;

namespace Blog.ViewModels.Posts
{
    public class PostViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Slug { get; set; }
        public DateTime LastUpdateDate { get; set; }

        public AuthorViewModel Author { get; set; }
        public CategoryViewModel Category { get; set; }
    }
}
