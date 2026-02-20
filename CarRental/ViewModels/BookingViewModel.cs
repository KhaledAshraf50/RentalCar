using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CarRental.ViewModels
{
    public class BookingViewModel
    {
        public int BookingId { get; set; }
        public int CarId { get; set; }
        [Required]
        [Display(Name = "Pickup Date")]
        [DataType(DataType.Date)]
        public DateTime PickupDate { get; set; }
        [Required]
        [Display(Name = "Return Date")]
        [DataType(DataType.Date)]
        public DateTime ReturnDate { get; set; }
        [Required]
        [Display(Name = "Pickup Location")]
        public int PickupLocationId { get; set; }

        [Required]
        [Display(Name = "Return Location")]
        public int ReturnLocationId { get; set; }
        [Display(Name = "Special Notes")]
        [MaxLength(1000)]
        public string? Notes { get; set; }
        public string? BookingReference { get; set; }
        public decimal TotalPrice { get; set; }
        public string? CarBrand { get; set; }
        public string? CarModel { get; set; }
        public string? CarImageUrl { get; set; }
        public string? Status { get; set; }
        public List<SelectListItem>? Locations {  get; set; }
    }
}
