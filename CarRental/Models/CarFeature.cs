using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRental.Models
{
    public class CarFeature
    {
        [Key]
        public int CarFeatureId { get; set; }
        [Required]
        [ForeignKey("Car")]
        public int CarId { get; set; }
        [Required]
        [ForeignKey("Feature")]
        public int FeatureId { get; set; }
        // Navigation Properties
        public virtual Car Car { get; set; } = null!;
        public virtual Feature Feature { get; set; } = null!;
    }
}
