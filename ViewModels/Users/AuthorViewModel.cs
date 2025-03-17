using Blog.ViewModels.Roles;

namespace Blog.ViewModels.Users
{
    public class AuthorViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Slug { get; set; }
        public List<RoleViewModel> Roles { get; set; }
    }
}
