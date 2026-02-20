using CarRental.Models;

namespace CarRental.Reposatory.Interfaces
{
    public interface ICarReposatory:IReposatory<Car>
    {
        //Custom Method For Car
        Task<IEnumerable<Car>> GetAllCarsAsync();
        Task<IEnumerable<Car>> GetAvailableCarAsync();
        Task<IEnumerable<Car>> GetCarByCategoryAsync(int categoryId);
        Task<IEnumerable<Car>> GetCarByLocationAsync(int locationId);
        Task<IEnumerable<Car>> GetFeaturedCarsAsync(int count = 6);
        Task<Car?> GetCarWithDetailsAsync(int id);
        Task UpdateCarFeaturesAsync(int carId, List<int> featureIds);

    }
}
