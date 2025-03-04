using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Enter E-mail")]
        [EmailAddress(ErrorMessage = "E-mail is invalid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter the Password")]
        public string Password { get; set; }
    }
}