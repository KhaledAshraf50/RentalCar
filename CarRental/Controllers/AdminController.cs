using CarRental.Models;
using CarRental.Reposatory.Interfaces;
using CarRental.Service.Interfaces;
using CarRental.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CarRental.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ICarService _carService;
        private readonly IUserService _userService;
        private readonly IBookingService _bookingService;
        private readonly ICategoryService _categoryService;
        private readonly ILocationService _locationService;
        private readonly IFeatureService _featureService;
        private readonly IFeatureRepo _featureRepository;
        private readonly IReposatory<Category> _categoryRepository;
        private readonly IReposatory<Location> _locationRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly ITestimonialReposatory _testimonialReposatory;
        private readonly ICarRequestService _carRequestService;


        public AdminController(
            ICarService carService,
            IUserService userService,
            IBookingService bookingService,
            ICategoryService categoryService,
            ILocationService locationService,
            IFeatureService featureService,
            IFeatureRepo featureRepository,
            IReposatory<Category> categoryRepository,
            IReposatory<Location> locationRepository,
            ITestimonialReposatory testimonialReposatory,
            IReviewRepository reviewRepository,
            ICarRequestService carRequestService)
        {
            _carService = carService;
            _userService = userService;
            _bookingService = bookingService;
            _categoryService = categoryService;
            _locationService = locationService;
            _featureService = featureService;
            _featureRepository = featureRepository;
            _categoryRepository = categoryRepository;
            _locationRepository = locationRepository;
            _reviewRepository = reviewRepository;
            _testimonialReposatory = testimonialReposatory;
            _carRequestService = carRequestService;
        }
        //Get: /Admin
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Main()
        {
            //var userRole = HttpContext.Session.GetString("UserRole");
            //if (userRole != "Admin")
            //{
            //    return RedirectToAction("Index", "HomePage");
            //}
            var model = new DashboardViewModel()
            {
                TotalCars = await _carService.CountAsync(),
                TotalBookings = await _bookingService.GetTotalBookingsCountAsync(),
                PendingBookings = await _bookingService.GetPendingBookingsCountAsync(),
                CompletedBookings = await _bookingService.GetCompletedBookingsCountAsync(),
                TotalUsers = await _userService.GetTotalUsersCountAsync(),
                MonthlyRevenue = await _bookingService.GetMonthlyRevenueAsync(DateTime.Now.Month, DateTime.Now.Year),
                RecentBookings = await _bookingService.GetRecentBookingsAsync(5),
            };
            return View("mainPageDashboard",model);
        }
        // Get: /Admin/AddCar
        public async Task<IActionResult> AddCar()
        {
            //var userRole = HttpContext.Session.GetString("UserRole");
            //if (userRole != "Admin")
            //{
            //    return RedirectToAction("Index", "HomePage");
            //}
            var model = new CarViewModel
            {
                Categories = await GetCategoriesSelectList(),
                Locations = await GetLocationsSelectList(),
                Features = await GetFeaturesSelectList(),
            };
            return View(model);
        }
        // POST: /Admin/AddCar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCar(CarViewModel modell)
        {
            //var userRole = HttpContext.Session.GetString("UserRole");
            //if (userRole != "Admin")
            //{
            //    return RedirectToAction("Index", "HomePage");
            //}

            if (!ModelState.IsValid)
            {
                modell.Categories = await GetCategoriesSelectList();
                modell.Locations = await GetLocationsSelectList();
                modell.Features = await GetFeaturesSelectList();
                return View(modell);
            }
            try
            {
                var car = await _carService.CreateCarAsync(modell);
                TempData["SuccessMessage"] = $"Car '{car.Brand} {car.Model}' added successfully!";
                return RedirectToAction("ManageCar");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                modell.Categories = await GetCategoriesSelectList();
                modell.Locations = await GetLocationsSelectList();
                modell.Features = await GetFeaturesSelectList();
                return View(modell);
            }
        }
        // GET: /Admin/ManageCar
        public async Task<IActionResult> ManageCar()
        {
            //var userRole = HttpContext.Session.GetString("UserRole");
            //if (userRole != "Admin")
            //{
            //    return RedirectToAction("Index", "HomePage");
            //}
            var cars = await _carService.GetAllCarsAsync();
            return View(cars);
        }
        // GET: /Admin/EditCar/{id}
       
        public async Task<IActionResult> EditCar(int id)
        {
            //var userRole = HttpContext.Session.GetString("UserRole");
            //if (userRole != "Admin")
            //{
            //    return RedirectToAction("Index", "HomePage");
            //}

            var car = await _carService.GetCarByIdAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            var model = new CarViewModel
            {
                CarId = car.CarId,
                Brand = car.Brand,
                Model = car.Model,
                Year = car.Year,
                DailyPrice = car.DailyPrice,
                Description = car.Description,
                Transmission = car.Transmission,
                FuelType = car.FuelType,
                SeatingCapacity = car.SeatingCapacity,
                CategoryId = car.CategoryId,
                LocationId = car.LocationId,
                ImageUrl = car.ImageUrl,
                Categories = await GetCategoriesSelectList(),
                Locations = await GetLocationsSelectList(),
                Features = await GetFeaturesSelectList(),
                FeatureIds = car.CarFeatures.Select(cf => cf.FeatureId).ToList()
            };
            return View(model);
        }
        // POST: /Admin/EditCar/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCar(int id, CarViewModel modell)
        {
            //var userRole = HttpContext.Session.GetString("UserRole");
            //if (userRole != "Admin")
            //{
            //    return RedirectToAction("Index", "HomePage");
            //}

            if (!ModelState.IsValid)
            {
                modell.Categories = await GetCategoriesSelectList();
                modell.Locations = await GetLocationsSelectList();
                modell.Features = await GetFeaturesSelectList();
                return View(modell);
            }

            try
            {
                var car = await _carService.UpdateCarAsync(id, modell);
                TempData["SuccessMessage"] = $"Car '{car.Brand} {car.Model}' updated successfully!";
                return RedirectToAction("ManageCar");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                modell.Categories = await GetCategoriesSelectList();
                modell.Locations = await GetLocationsSelectList();
                modell.Features = await GetFeaturesSelectList();
                return View(modell);
            }
        }
        // POST: /Admin/DeleteCar/{id}
        [HttpGet]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCar(int id)
        {
            //var userRole = HttpContext.Session.GetString("UserRole");
            //if (userRole != "Admin")
            //{
            //    return Json(new { success = false, message = "Unauthorized" });
            //}

            var success = await _carService.DeleteCarAsync(id);
            var car = await _carService.GetCarByIdAsync(id);
            if(car!.IsAvailable)
                TempData["SuccessMessage"] = $"Car '{car?.Brand} {car?.Model}' returned successfully!";
            else
                TempData["SuccessMessage"] = $"Car '{car?.Brand} {car?.Model}' deleted successfully!";
            return RedirectToAction("ManageCar");
        }

        // GET: /Admin/ManageBooking
        public async Task<IActionResult> ManageBooking()
        {
            //var userRole = HttpContext.Session.GetString("UserRole");
            //if (userRole != "Admin")
            //{
            //    return RedirectToAction("Index", "HomePage");
            //}

            var bookings = await _bookingService.GetAllBookingsAsync();
            return View(bookings);
        }
        public async Task<IActionResult> BookDetails(int id)
        {
            //var userRole = HttpContext.Session.GetString("UserRole");
            //if (userRole != "Admin")
            //{
            //    return RedirectToAction("Index", "HomePage");
            //}
            var book = await _bookingService.GetBookingWithDetailsAsync(id);
            return View(book);
        }
        // POST: /Admin/UpdateBookingStatus
        [HttpPost]
        public async Task<IActionResult> UpdateBookingStatus(int bookingId, string status)
        {
            //var userRole = HttpContext.Session.GetString("UserRole");
            //if (userRole != "Admin")
            //{
            //    return Json(new { success = false, message = "Unauthorized" });
            //}
            var success = await _bookingService.UpdateBookingStatusAsync(bookingId, status);
            return Json(new { success = success });
        }
        // GET: /Admin/ManageUsers
        public async Task<IActionResult> ManageUsers()
        {
            //var userRole = HttpContext.Session.GetString("UserRole");
            //if (userRole != "Admin")
            //{
            //    return RedirectToAction("Index", "HomePage");
            //}

            var users = await _userService.GetAllAsunc();
            return View(users);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeUserRole(int userId, string role)
        {
            //var userRole = HttpContext.Session.GetString("UserRole");
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != "Admin") return Json(new { success = false, message = "Unauthorized" });

            var success = await _userService.ChangeUserRoleAsync(userId, role);
            return Json(new { success });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleUserStatus(int userId)
        {
            //var userRole = HttpContext.Session.GetString("UserRole");
            //if (userRole != "Admin") return Json(new { success = false, message = "Unauthorized" });
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != "Admin") return Json(new { success = false, message = "Unauthorized" });
            var success = await _userService.ToggleUserStatusAsync(userId);
            return Json(new { success });
        }

        // GET: /Admin/ManageCategories
        public async Task<IActionResult> ManageCategories()
        {
            //var userRole = HttpContext.Session.GetString("UserRole");
            //if (userRole != "Admin")
            //{
            //    return RedirectToAction("Index", "HomePage");
            //}
            var categories = await _categoryService.GetCategoriesWithCountAsync();
            return View(categories);
        }
        // GET: /Admin/AddCategory
        public IActionResult AddCategory()
        {
            //var userRole = HttpContext.Session.GetString("UserRole");
            //if (userRole != "Admin")
            //    return RedirectToAction("Index", "HomePage");

            return View();
        }
        // POST: /Admin/AddCategory
        [HttpPost]
        public async Task<IActionResult> AddCategory(CreateCategoryVM model)
        {
            //var userRole = HttpContext.Session.GetString("UserRole");
            //if (userRole != "Admin")
            //{
            //    return Json(new { success = false, message = "Unauthorized" });
            //}

            if (!ModelState.IsValid)
                return View(model);
            var category = new Category
            {
                CategoryName = model.CategoryName,
                Description = model.Description
            };
            await _categoryService.CreateAsync(category);

            TempData["SuccessMessage"] = "Category added successfully!";
            return RedirectToAction("ManageCategories");
        }
        public  async Task<IActionResult> EditCategory(int id)
        {
            //var userRole = HttpContext.Session.GetString("UserRole");
            //if (userRole != "Admin")
            //    return RedirectToAction("Index", "HomePage");
            var category = await _categoryService.GetByIdAsync(id);
            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> EditCategory(int CategoryId, CreateCategoryVM model)
        {
            //var userRole = HttpContext.Session.GetString("UserRole");
            //if (userRole != "Admin")
            //    return RedirectToAction("Index", "HomePage");
            if (!ModelState.IsValid)
                return View(model);
            var category = new Category
            {
                CategoryId = CategoryId,
                CategoryName = model.CategoryName,
                Description = model.Description
            };
            await _categoryService.UpdateAsync(CategoryId,category);
            TempData["SuccessMessage"] = "Category Edited successfully!";
            return RedirectToAction("ManageCategories");
        }
        // POST: /Admin/DeleteCategory
        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            //var userRole = HttpContext.Session.GetString("UserRole");
            //if (userRole != "Admin")
            //{
            //    return Json(new { success = false, message = "Unauthorized" });
            //}

            var success = await _categoryService.DeleteAsync(id);
            TempData["SuccessMessage"] = "Category Deleted successfully!";
            return RedirectToAction("ManageCategories");
        }
        // GET: /Admin/ManageLocations
        public async Task<IActionResult> ManageLocations()
        {
            //var userRole = HttpContext.Session.GetString("UserRole");
            //if (userRole != "Admin")
            //{
            //    return RedirectToAction("Index", "HomePage");
            //}
            var locations = await _locationService.GetActiveLocationsViewAsync();
            return View(locations);
        }
        public IActionResult AddLocation()
        {
            //var userRole = HttpContext.Session.GetString("UserRole");
            //if (userRole != "Admin")
            //    return RedirectToAction("Index", "HomePage");

            return View();
        }
        // POST: /Admin/AddCategory
        [HttpPost]
        public async Task<IActionResult> AddLocation(LocationsWithCountVM model)
        {
            //var userRole = HttpContext.Session.GetString("UserRole");
            //if (userRole != "Admin")
            //{
            //    return Json(new { success = false, message = "Unauthorized" });
            //}

            if (!ModelState.IsValid)
                return View(model);
            var location = new Location
            {
                LocationName = model.LocationName,
                Address = model.Address,
                City = model.City,
                IsActive = model.IsActive
            };
            await _locationRepository.AddAsync(location);

            TempData["SuccessMessage"] = "Location added successfully!";
            return RedirectToAction("ManageLocations");
        }
        public async Task<IActionResult> EditLocation(int id)
        {
            //var userRole = HttpContext.Session.GetString("UserRole");
            //if (userRole != "Admin")
            //    return RedirectToAction("Index", "HomePage");
            var location = await _locationRepository.GetByIDAsync(id);
            var model = new LocationsWithCountVM
            {
                LocationId = location.LocationId,
                LocationName = location.LocationName,
                Address = location.Address,
                City = location.City,
                IsActive = location.IsActive,
            };
            return View(model);
        }
        [HttpPost]
        public  IActionResult EditLocation(int locationId, LocationsWithCountVM model)
        {
            //var userRole = HttpContext.Session.GetString("UserRole");
            //if (userRole != "Admin")
            //    return RedirectToAction("Index", "HomePage");
            if (!ModelState.IsValid)
                return View(model);
            var location = new Location
            {
                LocationId = locationId,
                LocationName = model.LocationName!,
                Address = model.Address!,
                City = model.City!,
                IsActive = model.IsActive,
            };
             _locationRepository.Update(location);
            TempData["SuccessMessage"] = "Location Edited successfully!";
            return RedirectToAction("ManageLocations");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteLocation(int id)
        {
            //var userRole = HttpContext.Session.GetString("UserRole");
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != "Admin")
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var success = await _locationService.DeleteAsync(id);
            TempData["SuccessMessage"] = "Location Deleted successfully!";
            return RedirectToAction("ManageLocations");
        }
        // GET: /Admin/ManageFeatures
        public async Task<IActionResult> ManageFeatures()
        {
            //var userRole = HttpContext.Session.GetString("UserRole");
            //if (userRole != "Admin")
            //{
            //    return RedirectToAction("Index", "HomePage");
            //}

            var features = await _featureRepository.GetFeaturesWithCarCount();
            return View(features);
        }
        public IActionResult AddFeature()
        {
            //var userRole = HttpContext.Session.GetString("UserRole");
            //if (userRole != "Admin")
            //    return RedirectToAction("Index", "HomePage");

            return View();
        }
        // POST: /Admin/AddCategory
        [HttpPost]
        public async Task<IActionResult> AddFeature(FeatureWithCountVM model)
        {
            //var userRole = HttpContext.Session.GetString("UserRole");
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != "Admin")
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            if (!ModelState.IsValid)
                return View(model);
            var feature = new Feature
            {
                FeatureName = model.FeatureName
            };
            await _featureRepository.AddAsync(feature);

            TempData["SuccessMessage"] = "Feature added successfully!";
            return RedirectToAction("ManageFeatures");
        }
        public async Task<IActionResult> EditFeature(int id)
        {
            //var userRole = HttpContext.Session.GetString("UserRole");
            //if (userRole != "Admin")
            //    return RedirectToAction("Index", "HomePage");
            var feature = await _featureRepository.GetByIDAsync(id);
            var model = new FeatureWithCountVM
            {
                FeatureId = feature!.FeatureId,
                FeatureName = feature.FeatureName
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult EditFeature(int featureId, FeatureWithCountVM model)
        {
            //var userRole = HttpContext.Session.GetString("UserRole");
            //if (userRole != "Admin")
            //    return RedirectToAction("Index", "HomePage");
            if (!ModelState.IsValid)
                return View(model);
            var feature = new Feature
            {
                FeatureId = featureId,
                FeatureName = model.FeatureName,
            };
            _featureRepository.Update(feature);
            TempData["SuccessMessage"] = "Feature Edited successfully!";
            return RedirectToAction("ManageFeatures");
        }

        public async Task<IActionResult> DeleteFeature(int id)
        {
            //var userRole = HttpContext.Session.GetString("UserRole");
            //if (userRole != "Admin")
            //{
            //    return Json(new { success = false, message = "Unauthorized" });
            //}

            var success = await _featureService.DeleteAsync(id);
            TempData["SuccessMessage"] = "Feature Deleted successfully!";
            return RedirectToAction("ManageFeatures");
        }

        // GET: /Admin/ManageReviews
        public async Task<IActionResult> ManageReviews(string? filter,string? search)
        {
            //var userRole = HttpContext.Session.GetString("UserRole");
            //if (userRole != "Admin")
            //{
            //    return RedirectToAction("Index", "HomePage");
            //}

            var reviews = await _reviewRepository.GetReviewsForAdminAsync(filter,search);
            ViewBag.CurrentFilter = filter;
            ViewBag.CurrentSearch = search;
            return View(reviews);
        }
        // POST: /Admin/ApproveReview
        [HttpPost]
        public async Task<IActionResult> ApproveReview(int id)
        {
            //var userRole = HttpContext.Session.GetString("UserRole");
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != "Admin")
            {
                return Json(new { success = false, message = "Unauthorized" });
            }
            //var adminId = HttpContext.Session.GetInt32("UserId");
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int adminId))
            {
                return RedirectToAction("Logout", "Account");
            }
            if (adminId == null)
                return RedirectToAction("ManageReviews");
            await _reviewRepository.ApproveReviewAsync(id, adminId);
            return RedirectToAction("ManageReviews");
        }
        [HttpPost]
        public async Task<IActionResult> RejectReview(int id)
        {
            await _reviewRepository.RejectReviewAsync(id);

            return RedirectToAction("ManageReviews");
        }

        // Helper methods
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

        // GET: /User/Profile - User Profile
        [Route("/Admin/Profile")]
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
        public async Task<IActionResult> PendingTestimonials()
        {
            var testimonials = await _testimonialReposatory.GetPendingTestimonialsAsync();
            return View(testimonials);
        }
        [HttpPost]
        public async Task<IActionResult> ApproveTestimonial(int id)
        {
            await _testimonialReposatory.ApproveTestimonialAsync(id);
            return RedirectToAction("PendingTestimonials");
        }
        [HttpPost]
        public async Task<IActionResult> RejectTestimonial(int id)
        {
            await _testimonialReposatory.RemoveTestimonialAsync(id);
            return RedirectToAction("PendingTestimonials");
        }
        public async Task<IActionResult> PendingCarRequests()
        {
            var requests = await _carRequestService.GetPendingRequestsAsync();
            return View(requests);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveCarRequest(int CarRequestId)
        {
            await _carRequestService.ApproveRequestAsync(CarRequestId);
            return RedirectToAction("PendingCarRequests");
        }

    }
}
