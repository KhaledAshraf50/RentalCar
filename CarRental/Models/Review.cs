using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRental.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        [Required]
        [ForeignKey("Booking")]
        public int BookingId { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        [ForeignKey("Car")]
        public int CarId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(2000)]
        public string? Comment { get; set; }

        [ForeignKey("ApprovedBy")]
        public int? ApprovedByUserId { get; set; }

        public DateTime? ApprovedAt { get; set; }
        public bool IsApproved { get; set; } = false;
        public bool IsActive { get; set; } = true;

        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual Booking Booking { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual Car Car { get; set; } = null!;

        [ForeignKey("ApprovedByUserId")]
        public virtual User? ApprovedBy { get; set; }
    }
}