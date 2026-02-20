using CarRental.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

public class CarFilterViewModel
{
    public IEnumerable<Car> Cars { get; set; } = new List<Car>();

    public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Locations { get; set; } = new List<SelectListItem>();

    public string? Search { get; set; }
    public int? CategoryId { get; set; }
    public int? LocationId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }

    public List<int> FavoriteIds { get; set; } = new();
}
