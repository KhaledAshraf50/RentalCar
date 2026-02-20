using CarRental.Models;
using CarRental.Reposatory.Implementions;

namespace CarRental.Reposatory.Interfaces
{
    public interface IFavoriteRepository:IReposatory<Favorite>
    {
        Task<List<int>> GetUserFavoriteCarIdsAsync(int userId);
        Task<bool> ExistsAsync(int userId, int carId);
         new Task AddAsync(Favorite favorite);
        Task RemoveAsync(Favorite favorite);
        Task<Favorite?> GetAsync(int userId, int carId);
    }
}
