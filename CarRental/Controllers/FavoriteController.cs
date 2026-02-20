using CarRental.Controllers;
using CarRental.Models;
using CarRental.Reposatory.Interfaces;
using CarRental.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public class FavoriteController : Controller
{
    private readonly IFavoriteService _favoriteService;
    private readonly IFavoriteRepository _favoriteRepository;
    private readonly ICarService _carService;
    private readonly ILogger<FavoriteController> _logger;



    public FavoriteController
        (IFavoriteService favoriteService,
        IFavoriteRepository favoriteRepo,
        ICarService carService,
        ILogger<FavoriteController> logger)
    {
        _favoriteService = favoriteService;
        _favoriteRepository = favoriteRepo;
        _carService = carService;
        _logger = logger;
    }
    // GET: /User/MyFavorite - User Favorites
    public async Task<IActionResult> MyFavorite()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdClaim, out int userId))
        {
            return RedirectToAction("Logout", "Account");
        }

        var favorites = await _favoriteRepository.FindAsync(f => f.UserId == userId);

        // Include Car details with each favorite
        var favoritesWithCars = favorites.Select( f =>
        {
            f.Car =  _carService.GetCarWithDetailsAsync(f.CarId).Result;
            return f;
        }).Where(f => f.Car != null && f.Car.IsActive).ToList();

        return View(favoritesWithCars);
    }

    // POST: /User/AddFavorite - Add to favorites
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddFavorite(int carId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdClaim, out int userId))
        {
            return RedirectToAction("Logout", "Account");
        }
        try
        {
            // Check if already in favorites
            var existing = await _favoriteRepository.FindAsync(f => f.UserId == userId && f.CarId == carId);
            if (existing.Any())
            {
                return Json(new { success = false, message = "Car already in favorites" });
            }

            var favorite = new Favorite
            {
                UserId = userId,
                CarId = carId,
                AddedAt = DateTime.UtcNow
            };

            await _favoriteRepository.AddAsync(favorite);
            return Json(new { success = true, message = "Added to favorites" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding favorite");
            return Json(new { success = false, message = "An error occurred" });
        }
    }

    // POST: /User/RemoveFavorite - Remove from favorites
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveFavorite(int favoriteId, int carId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdClaim, out int userId))
        {
            return RedirectToAction("Logout", "Account");
        }
        if (userId == null)
        {
            return Json(new { success = false, message = "Please login first" });
        }

        try
        {
            Favorite favorite;
            if (favoriteId > 0)
            {
                favorite = await _favoriteRepository.GetByIDAsync(favoriteId);
            }
            else
            {
                var favorites = await _favoriteRepository.FindAsync(f => f.UserId == userId && f.CarId == carId);
                favorite = favorites.FirstOrDefault();
            }

            if (favorite == null || favorite.UserId != userId)
            {
                return Json(new { success = false, message = "Favorite not found" });
            }

            _favoriteRepository.Delete(favorite);
            return Json(new { success = true, message = "Removed from favorites" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing favorite");
            return Json(new { success = false, message = "An error occurred" });
        }
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int carId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdClaim, out int userId))
        {
            return RedirectToAction("Logout", "Account");
        }

        if (userId == null)
            return RedirectToAction("Login", "Account");

        await _favoriteService.ToggleAsync(userId, carId);

        return Redirect(Request.Headers["Referer"].ToString());
    }
}
