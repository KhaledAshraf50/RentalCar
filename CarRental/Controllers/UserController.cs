using CarRental.Models;
using CarRental.Reposatory.Interfaces;
using CarRental.Service.Implementation;
using CarRental.Service.Interfaces;
using CarRental.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CarRental.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IBookingService _bookingService;
        private readonly IReposatory<Review> _reviewRepository;
        private readonly ICarService _carService;
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly ITestimonialReposatory _testimonialReposatory;
        private readonly ILogger<UserController> _logger;
        private readonly ICarRequestService _carRequestService;
        private readonly ICategoryService _categoryService;
        private readonly ILocationService _locationService;
        private readonly IFeatureRepo _featureRepository;




        public UserController(
            IUserService userService,
            IBookingService bookingService,
            IReposatory<Review> reviewRepository,
            IFavoriteRepository favoriteRepository,
            ICarService carService,
            ILogger<UserController> logger,
            ITestimonialReposatory testimonialReposatory,
            ICarRequestService carRequestService,
            ICategoryService categoryService,
            ILocationService locationService,
            IFeatureRepo featureRepository)
        {
            _userService = userService;
            _bookingService = bookingService;
            _reviewRepository = reviewRepository;
            _carService = carService;
            _logger = logger;
            _favoriteRepository = favoriteRepository;
            _testimonialReposatory = testimonialReposatory;
            _carRequestService = carRequestService;
            _categoryService = categoryService;
            _locationService = locationService;
            _featureRepository = featureRepository;
        }

        // GET: /User/MainPage - User Dashboard
        [Authorize]
        public async Task<IActionResult> MainPage()
        {
            //var userId = HttpContext.Session.GetInt32("UserId");
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToAction("Logout", "Account");
            }
            //if (userId == null)
            //{
            //    return RedirectToAction("Login", "Account");
            //}

            var user = await _userService.GetByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Logout", "Account");
            }

            // Get user's bookings
            var bookings = await _bookingService.GetUserBookingsAsync(userId);

            // Calculate dashboard stats
            var activeBookings = bookings.Count(b => b.Status == "Pending" || b.Status == "Confirmed");
            var completedTrips = bookings.Count(b => b.Status == "Completed");
            var totalSpent = bookings.Where(b => b.Status == "Completed").Sum(b => b.TotalPrice);
            var recentBookings = bookings.OrderByDescending(b => b.BookingDate).Take(5).ToList();

            var dashboardModel = new DashboardViewModel
            {
                ActiveBookings = activeBookings,
                CompletedTrips = completedTrips,
                TotalSpent = totalSpent,
                UserRecentBookings = recentBookings
            };

            ViewBag.User = user;
            return View(dashboardModel);
        }

        // GET: /User/Profile - User Profile
        public async Task<IActionResult> Profile()
        {
            //var userId = HttpContext.Session.GetInt32("UserId");
            //if (userId == null)
            //{
            //    return RedirectToAction("Login", "Account");
            //}
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToAction("Logout", "Account");
            }

            var user = await _userService.GetByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Logout", "Account");
            }

            var model = new UserViewModel
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                DateOfBirth = user.DateOfBirth,
                Address = user.Address,
                ProfileImage = user.ProfileImage,
                Role = user.Role,
            };
            //HttpContext.Session.SetString("ProfileImage", model.ProfileImage);
            var claim = new Claim("ProfileImage", model.ProfileImage ?? "");    
            return View(model);
        }

        // POST: /User/UpdateProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UserViewModel model)
        {
            //var userId = HttpContext.Session.GetInt32("UserId");
            //if (userId == null)
            //{
            //    return RedirectToAction("Login", "Account");
            //}
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToAction("Logout", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View("Profile", model);
            }

            try
            {
                var success = await _userService.UpdateUserProfileAsync(userId, model);
                if (success)
                {
                    TempData["SuccessMessage"] = "Profile updated successfully";
                    var user = await _userService.GetByIdAsync(userId);

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(ClaimTypes.Name, user.FullName),
                        new Claim(ClaimTypes.Role, user.Role),
                        new Claim("ProfileImage", user.ProfileImage ?? "")
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true
                    };

                    await HttpContext.SignInAsync("Cookies",
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToAction("Profile");
                }

                ModelState.AddModelError("", "Failed to update profile");
                return View("Profile", model);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("Email", ex.Message);
                return View("Profile", model);
            }
        }

        // POST: /User/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(UserViewModel model)
        {
            //var userId = HttpContext.Session.GetInt32("UserId");
            //if (userId == null)
            //{
            //    return RedirectToAction("Login", "Account");
            //}
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToAction("Logout", "Account");
            }

            if (string.IsNullOrEmpty(model.CurrentPassword) || string.IsNullOrEmpty(model.NewPassword))
            {
                ModelState.AddModelError("", "Current and new password are required");
                var user = await _userService.GetByIdAsync(userId);
                var userModel = new UserViewModel
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = user.Phone,
                    DateOfBirth = user.DateOfBirth,
                    Address = user.Address,
                    ProfileImage = user.ProfileImage
                };
                return View("Profile", userModel);
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match");
                var user = await _userService.GetByIdAsync(userId);
                var userModel = new UserViewModel
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = user.Phone,
                    DateOfBirth = user.DateOfBirth,
                    Address = user.Address,
                    ProfileImage = user.ProfileImage
                };
                return View("Profile", userModel);
            }

            var success = await _userService.ChangePasswordAsync(userId, model.CurrentPassword, model.NewPassword);
            if (success)
            {
                TempData["SuccessMessage"] = "Password changed successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Current password is incorrect";
            }

            return RedirectToAction("Profile");
        }
        // GET: /User/MyReview - User Reviews
        public async Task<IActionResult> MyReview()
        {
            //var userId = HttpContext.Session.GetInt32("UserId");
            //if (userId == null)
            //{
            //    return RedirectToAction("Login", "Account");
            //}
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToAction("Logout", "Account");
            }

            var reviews = await _reviewRepository.FindAsync(r => r.UserId == userId);

            // Include Car details with each review
            var reviewsWithCars = reviews.Select(r =>
            {
                r.Car = _carService.GetCarByIdAsync(r.CarId).Result;
                r.Booking = _bookingService.GetByIdAsync(r.BookingId).Result;
                return r;
            }).OrderByDescending(r => r.ReviewDate).ToList();

            return View(reviewsWithCars);
        }

        // GET: /User/EditReview/{id}
        public async Task<IActionResult> EditReview(int id)
        {
            //var userId = HttpContext.Session.GetInt32("UserId");
            //if (userId == null)
            //{
            //    return RedirectToAction("Login", "Account");
            //}
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToAction("Logout", "Account");
            }

            var review = await _reviewRepository.GetByIDAsync(id);
            if (review == null || review.UserId != userId)
            {
                return NotFound();
            }

            review.Car = await _carService.GetCarByIdAsync(review.CarId);
            review.Booking = await _bookingService.GetByIdAsync(review.BookingId);

            return View(review);
        }

        // POST: /User/EditReview
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditReview(int id, string comment)
        {
            //var userId = HttpContext.Session.GetInt32("UserId");
            //if (userId == null)
            //    return RedirectToAction("Login", "Account");
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToAction("Logout", "Account");
            }

            var review = await _reviewRepository.GetByIDAsync(id);

            if (review == null || review.UserId != userId)
                return NotFound();

            review.Comment = comment;

            review.IsApproved = false;
            review.ApprovedAt = null;
            review.ApprovedByUserId = null;

            _reviewRepository.Update(review);

            TempData["SuccessMessage"] =
                "Comment updated and pending approval";

            return RedirectToAction("MyReview");
        }
        // POST: /User/DeleteReview
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(int id)
        {
            //var userId = HttpContext.Session.GetInt32("UserId");
            //if (userId == null)
            //    return RedirectToAction("Login", "Account");
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToAction("Logout", "Account");
            }

            var review = await _reviewRepository.GetByIDAsync(id);

            if (review == null || review.UserId != userId)
                return NotFound();

            _reviewRepository.Delete(review);

            TempData["SuccessMessage"] = "Review deleted successfully";

            return RedirectToAction("MyReview");
        }

        // GET: /User/Statistics - User Statistics
        public async Task<IActionResult> Statistics()
        {
            //var userId = HttpContext.Session.GetInt32("UserId");
            //if (userId == null)
            //{
            //    return RedirectToAction("Login", "Account");
            //}
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToAction("Logout", "Account");
            }

            var bookings = await _bookingService.GetUserBookingsAsync(userId);
            var favorites = await _favoriteRepository.FindAsync(f => f.UserId == userId);
            var reviews = await _reviewRepository.FindAsync(r => r.UserId == userId);

            var stats = new
            {
                TotalBookings = bookings.Count(),
                CompletedBookings = bookings.Count(b => b.Status == "Completed"),
                CancelledBookings = bookings.Count(b => b.Status == "Cancelled"),
                TotalSpent = bookings.Where(b => b.Status == "Completed").Sum(b => b.TotalPrice),
                TotalFavorites = favorites.Count(),
                TotalReviews = reviews.Count(),
                AverageRating = reviews.Where(r => r.IsApproved).DefaultIfEmpty().Average(r => r?.Rating ?? 0),
                MostRentedCar = bookings.GroupBy(b => b.CarId)
                                       .OrderByDescending(g => g.Count())
                                       .Select(g => g.Key)
                                       .FirstOrDefault()
            };

            return Json(stats);
        }
        [Authorize]
        public IActionResult AddTestimonial()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddTestimonial(string comment, int rating)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToAction("Logout", "Account");
            }
            await _testimonialReposatory.AddTestimonialAsync(userId, comment, rating);

            TempData["SuccessMessage"] = "Your testimonial has been submitted and is awaiting approval.";

            return RedirectToAction("Profile");
        }
        [Authorize]
        public async Task<IActionResult> AddCarRequest()
        {
            var model = new CarViewModel
            {
                Categories = await GetCategoriesSelectList(),
                Locations = await GetLocationsSelectList(),
                Features = await GetFeaturesSelectList(),
            };

            return View("AddCarRequest", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddCarRequest(CarViewModel modell)
        {
            if (!ModelState.IsValid)
            {
                modell.Categories = await GetCategoriesSelectList();
                modell.Locations = await GetLocationsSelectList();
                modell.Features = await GetFeaturesSelectList();
                return View(modell);
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

          

            await _carRequestService.CreateCarRequestAsync(modell,userId);

            TempData["SuccessMessage"] = "Your car request has been submitted for review";
            return RedirectToAction("Profile");
        }

        private async Task<List<SelectListItem>> GetCategoriesSelectList()
        {
            var categories = await _categoryService.GetActiveCategoriesAsync();
            return categories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.CategoryName
            }).ToList();
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

        private async Task<List<SelectListItem>> GetFeaturesSelectList()
        {
            var features = await _featureRepository.GetAllAsync();
            return features.Select(f => new SelectListItem
            {
                Value = f.FeatureId.ToString(),
                Text = f.FeatureName
            }).ToList();
        }

    }
}