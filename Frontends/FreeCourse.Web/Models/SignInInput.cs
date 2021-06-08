using System.ComponentModel.DataAnnotations;

namespace FreeCourse.Web.Models
{
    public class SignInInput
    {
        [Display(Name = "Email adresiniz")]
        [Required]
        public string Email { get; set; }
        [Display(Name = "Şifreniz")]
        [Required]
        public string Password { get; set; }
        [Display(Name = "Beni hatırla")]
        public bool IsRemember { get; set; }
    }
}