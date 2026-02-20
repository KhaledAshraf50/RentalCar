using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRental.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        [ForeignKey("Car")]
        public int CarId { get; set; }

        [Required]
        [ForeignKey("PickupLocation")]
        public int PickupLocationId { get; set; }

        [Required]
        [ForeignKey("ReturnLocation")]
        public int ReturnLocationId { get; set; }

        [MaxLength(50)]
        public string BookingReference { get; set; } = GenerateBookingReference();

        [Required]
        [DataType(DataType.Date)]
        public DateTime PickupDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ReturnDate { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        [MaxLength(500)]
        public string? CancellationReason { get; set; }

        public DateTime? CompletedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? CancelledAt { get; set; }

        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

        [MaxLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Completed, Cancelled

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        // Navigation Properties
        public virtual User User { get; set; } = null!;
        public virtual Car Car { get; set; } = null!;

        [ForeignKey("PickupLocationId")]
        public virtual Location PickupLocation { get; set; } = null!;

        [ForeignKey("ReturnLocationId")]
        public virtual Location ReturnLocation { get; set; } = null!;

        public virtual Review? Review { get; set; }

        // دالة مساعدة لتوليد رقم الحجز
        private static string GenerateBookingReference()
        {
            return $"CAR{DateTime.Now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";
        }
    }
}