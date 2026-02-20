using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CarRental.ViewModels
{
    public class CarViewModel
    {
        public int CarId { get; set; }
        [Required(ErrorMessage ="Brand Is Required")]
        [MaxLength(100)]
        public string Brand { get; set; }=string.Empty;
        [Required(ErrorMessage ="Model Is Required")]
        [MaxLength(100)]
        public string Model { get; set; }= string.Empty;
        [Required(ErrorMessage ="Year Is Required")]
        [Range(1900,2026,ErrorMessage ="Please Enter A Valid Year")]
        public int Year { get; set; }
        [Required(ErrorMessage ="Daily price Is Required")]
        [Display(Name ="Daily Price")]
        [Range(1,10000,ErrorMessage = "Price must be between $1 and $10,000")]
        public decimal DailyPrice { get; set; }
        [Display(Name = "Description")]
        [MaxLength(500)]
        public string? Description { get; set; }
        [Required]
        [Display(Name = "Transmission")]
        public string Transmission { get; set; } = "Automatic";
        [Required]
        [Display(Name = "Fuel Type")]
        public string FuelType { get; set; } = "Gasoline";
        [Required]
        [Display(Name = "Seating Capacity")]
        [Range(1, 20, ErrorMessage = "Seating capacity must be between 1 and 20")]
        public int SeatingCapacity { get; set; } = 5;
        [Display(Name = "Car Image")]
        public IFormFile? ImageFile { get; set; }
        public string? ImageUrl { get; set; }
        [Display(Name = "Category")]
        [Required(ErrorMessage = "Please select a category")]
        public int CategoryId { get; set; }
        [Display(Name = "Location")]
        [Required(ErrorMessage = "Please select a location")]
        public int LocationId { get; set; }
        [Display(Name = "Features")]
        public List<int> FeatureIds { get; set; }=new List<int>();
        // For dropdowns in the view
        public List<SelectListItem>? Categories { get; set; }
        public List<SelectListItem>? Locations { get; set; }
        public List<SelectListItem>? Features { get; set; }
        public List<SelectListItem>? Transmissions { get; set; } = new ()
        {
            new SelectListItem{Value="Automatic",Text="Automatic" },
            new SelectListItem{Value="Manual",Text="Manual"},
            new SelectListItem{Value="Semi-Automatic",Text="Semi-Automatic"}
        };
        public List<SelectListItem>? FuelTypes { get; set; } = new ()
        {
            new SelectListItem{Value="Gasoline",Text="Gasoline"},
            new SelectListItem{Value="Diesel",Text="Diesel"},
            new SelectListItem{Value="Electric",Text="Electric"},
            new SelectListItem{Value="Hybrid",Text="Hybrid"},
        };
    }
}
