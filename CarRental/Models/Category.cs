using System.ComponentModel.DataAnnotations;

namespace CarRental.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Required]
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; }
        // Navigation Properties
        public virtual ICollection<Car>? Cars { get; set; }
        public virtual ICollection<Location>? Locations { get; set; }
    }
}
