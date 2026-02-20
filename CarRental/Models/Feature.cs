using System.ComponentModel.DataAnnotations;

namespace CarRental.Models
{
    public class Feature
    {
        [Key]
        public int FeatureId { get; set; }
        [Required]
        [MaxLength(100)]
        public string FeatureName { get; set; } = string.Empty;
        [MaxLength(50)]
        public string? Icon { get; set; }
        // Navigation Properties
        public virtual ICollection<CarFeature> CarFeatures { get; set; } = new List<CarFeature>();
    }
}
