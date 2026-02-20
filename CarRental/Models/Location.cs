using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRental.Models
{
    public class Location
    {
        [Key]
        public int LocationId { get; set; }
        [Required]
        [MaxLength(100)]
        public string LocationName { get; set; } = string.Empty;
        [Required]
        [MaxLength(500)]
        public string Address { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        // Foreign Key
        [ForeignKey("Category")]
        public int? CategoryId { get; set; }
        // Navigation Properties
        public virtual Category Category { get; set; }
        public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
        public virtual ICollection<Booking> PickupBookings { get; set; } = new List<Booking>();
        public virtual ICollection<Booking> ReturnBookings { get; set; } = new List<Booking>();
    }
}
