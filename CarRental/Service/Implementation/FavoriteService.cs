using CarRental.Models;
using CarRental.Reposatory.Interfaces;

public class FavoriteService : IFavoriteService
{
    private readonly IFavoriteRepository _repo;

    public FavoriteService(IFavoriteRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<int>> GetUserFavoriteCarIds(int userId)
    {
        return await _repo.GetUserFavoriteCarIdsAsync(userId);
    }

    public async Task ToggleAsync(int userId, int carId)
    {
        var existing = await _repo.GetAsync(userId, carId);

        if (existing == null)
        {
            await _repo.AddAsync(new Favorite
            {
                UserId = userId,
                CarId = carId
            });
        }
        else
        {
            await _repo.RemoveAsync(existing);
        }
    }
}
