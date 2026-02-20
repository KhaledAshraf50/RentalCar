using System.ComponentModel.DataAnnotations;

namespace CarRental.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Role { get; set; } = "User"; // Default value

        public bool IsActive { get; set; } = true;

        [MaxLength(500)]
        public string? ProfileImage { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public virtual ICollection<Car> CarsAdded { get; set; } = new List<Car>(); // Cars added by this user
        public virtual ICollection<Review> ReviewsApproved { get; set; } = new List<Review>(); // Reviews approved by this admin
        public virtual ICollection<Car> CarsApproved { get; set; } = new List<Car>(); // Cars approved by this admin
        public virtual ICollection<Testimonial> Testimonial { get; set; } = new List<Testimonial>(); 
        public virtual ICollection<CarRequest> CarRequest { get; set; } = new List<CarRequest>(); 
    }
}