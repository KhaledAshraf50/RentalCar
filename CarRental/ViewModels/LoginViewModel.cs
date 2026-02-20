using System.ComponentModel.DataAnnotations;

namespace CarRental.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage ="Email Is Required")]
        [EmailAddress(ErrorMessage ="Invalid Email Address!!")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage ="Password Is Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }= string.Empty;
        [Display(Name ="Remember Me")]
        public bool RememberMe { get; set; }
    }
}
