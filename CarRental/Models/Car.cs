using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRental.Models
{
    public class Car
    {
        [Key]
        public int CarId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Brand { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Model { get; set; } = string.Empty;

        [Required]
        public int Year { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DailyPrice { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(50)]
        public string Transmission { get; set; } = "Automatic";

        [MaxLength(50)]
        public string FuelType { get; set; } = "Gasoline";

        public int SeatingCapacity { get; set; } = 5;

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public bool IsAvailable { get; set; } = true;
        public bool IsActive { get; set; } = true;
        public bool IsApproved { get; set; } = true; // For user-added cars

        [Column(TypeName = "decimal(3,2)")]
        public decimal? AverageRating { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovedAt { get; set; }

        // Foreign Keys
        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        [ForeignKey("Location")]
        public int LocationId { get; set; }

        [ForeignKey("AddedBy")]
        public int AddedByUserId { get; set; }

        [ForeignKey("ApprovedBy")]
        public int? ApprovedByUserId { get; set; }

        // Navigation Properties
        public virtual Category Category { get; set; } = null!;
        public virtual Location Location { get; set; } = null!;
        public virtual User AddedBy { get; set; } = null!;
        public virtual User? ApprovedBy { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<CarFeature> CarFeatures { get; set; } = new List<CarFeature>();
        public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    }
}