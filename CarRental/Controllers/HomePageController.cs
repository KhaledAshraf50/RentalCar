using CarRental.Reposatory.Interfaces;
using CarRental.Service.Implementation;
using CarRental.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace CarRental.Controllers
{
    public class HomePageController : Controller
    {
        private readonly ICarService _carService;
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly ILocationService _locationService;
        private readonly ITestimonialReposatory _testimonialReposatory;
        public HomePageController(ICarService carService,
                             IFavoriteRepository favoriteRepository,
                             ILocationService locationService,
                             ITestimonialReposatory testimonialReposatory)
        {
            _carService = carService;
            _favoriteRepository = favoriteRepository;
            _locationService = locationService;
            _testimonialReposatory = testimonialReposatory;
        }
        [AllowAnonymous]

        public async Task<IActionResult> Index()
        {
            var cars = await _carService.GetAvailableCarsAsync();
            var testimonials = await _testimonialReposatory.GetApprovedTestimonialsAsync();
            var featuredCars = cars.Take(6).ToList();
            //var userId = HttpContext.Session.GetInt32("UserId");

            List<int> favoriteIds = new();

             var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (int.TryParse(userIdClaim, out int userId))
                {
                    var favorites = await _favoriteRepository
                        .FindAsync(f => f.UserId == userId);

                    favoriteIds = favorites.Select(f => f.CarId).ToList();
                }
            
            var model = new HomePageViewModel
            {
                Cars = featuredCars,
                FavoriteIds = favoriteIds,
                Locations = await GetLocationsSelectList(),
                Testimonials = testimonials,
            };
            return View("HomePage",model);
        }
        private async Task<List<SelectListItem>> GetLocationsSelectList()
        {
            var locations = await _locationService.GetActiveLocationsAsync();
            return locations.Select(l => new SelectListItem
            {
                Value = l.LocationId.ToString(),
                Text = $"{l.LocationName} - {l.City}"
            }).ToList();
        }
    }
}
