using System.ComponentModel.DataAnnotations;

namespace StudentStorage.Models.Authentication
{
    public class LoginModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "User Name is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}
