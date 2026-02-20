using CarRental.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

public class HomePageViewModel
{
    public int LocationId { get; set; }
    public List<Car> Cars { get; set; } = new();
    public List<int> FavoriteIds { get; set; } = new();
    public List<SelectListItem>? Locations { get; set; }
    public List<Testimonial>? Testimonials { get; set; }
}
