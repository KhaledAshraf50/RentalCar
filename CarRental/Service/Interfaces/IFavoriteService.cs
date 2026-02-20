public interface IFavoriteService
{
    Task<List<int>> GetUserFavoriteCarIds(int userId);
    Task ToggleAsync(int userId, int carId);
}
