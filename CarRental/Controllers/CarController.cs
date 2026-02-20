using CarRental.Models;
using CarRental.Reposatory.Interfaces;
using CarRental.Service.Interfaces;
using CarRental.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CarRental.Controllers
{
    public class CarController : Controller
    {
        private readonly ICarService _carService;
        private readonly ICategoryService _categoryService;
        private readonly ILocationService _locationService;
        IFavoriteRepository _favoriteRepository;
        IReviewRepository _reviewRepository;
        private readonly IReposatory<Feature> _featureRepository;
        private readonly IBookingService _bookingService;
        private readonly ILogger<CarController> _logger;

        public CarController(
            ICarService carService,
            ICategoryService categoryService,
            ILocationService locationService,
            IReposatory<Feature> featureRepository,
            IBookingService bookingService,
            ILogger<CarController> logger,
            IFavoriteRepository favoriteRepository,
            IReviewRepository reviewRepository
            )
        {
            _carService = carService;
            _categoryService = categoryService;
            _locationService = locationService;
            _featureRepository = featureRepository;
            _bookingService = bookingService;
            _logger = logger;
            _favoriteRepository = favoriteRepository;
            _reviewRepository = reviewRepository;
        }
        // Get: /car (Available Cars)
        [AllowAnonymous]
        public async Task<IActionResult> Index
            (string? search,
            int? categoryId,
            int? locationId,
            decimal? minprice,
            decimal? maxprice)
        {
            var cars = await _carService.GetAvailableCarsAsync();
            if (!string.IsNullOrEmpty(search))
            {
                cars = cars.Where(c =>
                c.Brand.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                c.Model.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                c.Description.Contains(search, StringComparison.OrdinalIgnoreCase));
            }
            if (categoryId.HasValue && categoryId > 0)
            {
                cars = cars.Where(c => c.CategoryId == categoryId);
            }
            if (locationId.HasValue && locationId > 0)
            {
                cars = cars.Where(c => c.LocationId == locationId.Value);
            }
            if (minprice.HasValue)
            {
                cars = cars.Where(c => c.DailyPrice >= minprice.Value);
            }
            if (maxprice.HasValue)
            {
                cars = cars.Where(c => c.DailyPrice <= maxprice.Value);
            }
            var model = new CarFilterViewModel
            {
                Cars = cars,
                Categories = await GetCategoriesSelectList(),
                Locations = await GetLocationsSelectList(),
                Search = search,
                CategoryId = categoryId,
                LocationId = locationId,
                MinPrice = minprice,
                MaxPrice = maxprice,
            };
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
            {
                var favorites = await _favoriteRepository
      .FindAsync(f => f.UserId == userId);

                model.FavoriteIds = favorites
                    .Select(f => f.CarId)
                    .ToList();
            }
            return View("Cars", model);
        }
       [HttpGet("Car/CarDetails/{id:int}")]
        public async Task<IActionResult> CarDetails(int id)
        {
            var car = await _carService.GetCarByIdAsync(id);
            if (car == null)
            {
                return NotFound();
            }
            // Prepare booking view model
            var viewModel = new CarDetailsViewModel
            {
                Car = car,
                Booking = new BookingViewModel
                {
                    CarId = car.CarId,
                    Locations = await GetLocationsSelectList()
                },
                Review = new AddReviewViewModel
                {
                    CarId = car.CarId
                },
            };
            return View(viewModel);
        }
        async Task<List<SelectListItem>> GetCategoriesSelectList()
        {
            var Categories = await _categoryService.GetActiveCategoriesAsync();
            return Categories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.CategoryName
            }).ToList();
        }
        async Task<List<SelectListItem>> GetLocationsSelectList()
        {
            var locations = await _locationService.GetActiveLocationsAsync();
            return locations.Select(l => new SelectListItem
            {
                Value = l.LocationId.ToString(),
                Text = $"{l.LocationName} - {l.City}"
            }).ToList();
        }
        // post: /car/Book
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(
     [Bind(Prefix = "Booking")] BookingViewModel model)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Please login to book a car";
                return RedirectToAction("Login","Account");
            }
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage).ToList();
                return Json(new
                {
                    success = false,
                    message = string.Join(", ", errors)
                });
            }
             var booking = await _bookingService.CreateBookingAsync(model, userId.Value);
                TempData["SuccessMessage"] = $"Booking created successfully! Your reference number is: {booking.BookingReference}";
                return RedirectToAction("GetBookingUser");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking");
                TempData["ErrorMessage"] = "Something went wrong";
                return RedirectToAction("CarDetails", new { id = model.CarId });
            }
        }
        // GET: /Car/MyBookings
        public async Task<IActionResult> GetBookingUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToAction("Logout", "Account");
            }

            var bookings = await _bookingService.GetUserBookingsAsync(userId);
            return View("MyBooking", bookings);
        }
        // POST: /Car/CancelBooking/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelBooking(int id, string? reason)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToAction("Logout", "Account");
            }
            var success = await _bookingService.CancelBookingAsync(id, userId, reason);
            if (success)
            {
                TempData["SuccessMessage"] = "Booking cancelled successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to cancel booking";
            }
            return RedirectToAction("GetBookingUser");
        }
        // AJAX: Calculate rental price
        [HttpPost]
        public async Task<IActionResult> CalculatePrice(int carId, DateTime pickupDate, DateTime returnDate)
        {
            try
            {
                var price = await _bookingService.CalculateTotalPriceAsync(carId, pickupDate, returnDate);
                var car = await _carService.GetCarByIdAsync(carId);
                var days = (returnDate - pickupDate).Days;
                var subtotal = car.DailyPrice * days;
                var discount = subtotal - price;
                var discountpercentage = discount > 0 ? (discount/subtotal)*100: 0;
                return Json(new 
                {
                    success = true,
                    price = price ,
                    subtotal = subtotal,
                    discount = discount,
                    discountpercentage = Math.Round(discountpercentage,0),
                    days = days,
                    dailyPrice = car.DailyPrice
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview([Bind(Prefix = "Review")] AddReviewViewModel model)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToAction("Logout", "Account");
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid review data";
                return RedirectToAction("CarDetails", new { id = model.CarId });
            }
            var hasBooked = await _carService
                .HasUserBookedCarAsync(userId, model.CarId);
            if (!hasBooked)
            {
                TempData["ErrorMessage"] =
                    "You Can only review cars you Have booked";
                return RedirectToAction("CarDetails", new { id = model.CarId });
            }
            var hasReview = await _carService
                .HasUserReviewedCarAsync(userId, model.CarId);
            if (hasReview)
            {
                TempData["ErrorMessage"] =
                    "You Have Already Reviewed This Car";
                return RedirectToAction("CarDetails", new { id = model.CarId });
            }
            var bookingId = await _carService
    .GetCompletedBookingIdAsync(userId, model.CarId);

            if (bookingId == null)
            {
                TempData["ErrorMessage"] =
                    "You must complete a booking before reviewing";
                return RedirectToAction("CarDetails", new { id = model.CarId });
            }

            var review = new Review
            {
                BookingId = bookingId.Value,
                CarId = model.CarId,
                UserId = userId,
                Rating = model.Rating,
                Comment = model.Comment,
                ReviewDate = DateTime.Now,
                IsApproved = false
            };

            await _carService.AddReviewAsync(review);

            TempData["SuccessMessage"] = "Review submitted and waiting approval";

            return RedirectToAction("CarDetails", new { id = model.CarId });
        }
    }
}
