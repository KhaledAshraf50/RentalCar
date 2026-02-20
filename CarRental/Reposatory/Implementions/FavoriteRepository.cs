using CarRental.Models;
using CarRental.Models.DBContext;
using CarRental.Reposatory.Implementions;
using CarRental.Reposatory.Interfaces;
using Microsoft.EntityFrameworkCore;

public class FavoriteRepository :Reposatory<Favorite>, IFavoriteRepository
{
    private readonly CarRentalDbContext _context;

    public FavoriteRepository(CarRentalDbContext context):base(context)
    {
        _context = context;
    }

    public async Task<List<int>> GetUserFavoriteCarIdsAsync(int userId)
    {
        return await _context.Favorites
            .Where(f => f.UserId == userId)
            .Select(f => f.CarId)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(int userId, int carId)
    {
        return await _context.Favorites
            .AnyAsync(f => f.UserId == userId && f.CarId == carId);
    }

    public async Task<Favorite?> GetAsync(int userId, int carId)
    {
        return await _context.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.CarId == carId);
    }

    public async new Task AddAsync(Favorite favorite)
    {
        await _context.Favorites.AddAsync(favorite);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(Favorite favorite)
    {
        _context.Favorites.Remove(favorite);
        await _context.SaveChangesAsync();
    }
}
