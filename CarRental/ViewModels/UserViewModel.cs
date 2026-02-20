using System.ComponentModel.DataAnnotations;

namespace CarRental.ViewModels
{
    public class UserViewModel
    {
        public int UserId { get; set; }
        [Display(Name = "Full Name")]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;
        [Phone]
        [Display(Name = "Phone Number")]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        [Display(Name = "Address")]
        [MaxLength(500)]
        public string? Address { get; set; }
        [Display(Name = "Profile Image")]
        public IFormFile? ProfileImageFile { get; set; }
        public string? ProfileImage { get; set; }
        [Display(Name = "Role")]
        public string Role { get; set; } = "User";
        // For change password
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string? CurrentPassword { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string? NewPassword { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string? ConfirmPassword { get; set; }
    }
}
