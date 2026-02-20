using System.ComponentModel.DataAnnotations;

namespace CarRental.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage ="Full Name Is Required")]
        [Display(Name ="Full Name")]
        [MaxLength(100)]
        public string FullName { get; set; }=string.Empty;
        [Required(ErrorMessage ="Email Is Required")]
        [EmailAddress(ErrorMessage ="Invalid Email Address!!")]
        [MaxLength(100)]
        public string Email { get; set; }= string.Empty;
        [Required(ErrorMessage ="Phone Number Is Required")]
        [Phone(ErrorMessage ="Invalid Phone Number")]
        [Display(Name ="Phone Number")]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = string.Empty;
        [Required(ErrorMessage ="Confirm Password Is Required")]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage ="Passwords Do Not Match!!!")]
        public string ConfirmPassword { get; set; } = string.Empty;
        [Display(Name ="I agree to the Terms and Conditions")]
        [Range(typeof(bool),"true","true",ErrorMessage ="You Must Accept The Terms!!")]
        public bool AcceptTerms {  get; set; }
    }
}
