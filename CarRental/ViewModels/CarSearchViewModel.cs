using CarRental.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarRental.ViewModels
{
    public class CarSearchViewModel
    {
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public int? LocationId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public List<SelectListItem>? Categories { get; set; }
        public List<SelectListItem>? Locations { get; set; }
        public List<Car> Cars { get; set; } = new();
    }
}