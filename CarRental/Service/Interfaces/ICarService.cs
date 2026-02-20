using CarRental.Models;
using CarRental.ViewModels;

namespace CarRental.Service.Interfaces
{
    public interface ICarService:IService<Car>
    {
        // Get 
        Task<IEnumerable<Car>> GetAllCarsAsync();
        Task<Car?> GetCarByIdAsync(int id);
        Task<Car?> GetCarWithDetailsAsync(int id);
        // Search and Filter
        Task<IEnumerable<Car>> GetAvailableCarsAsync();
        Task<IEnumerable<Car>> GetFeaturedCarsAsync(int count = 6);
        Task<IEnumerable<Car>> GetCarsByCategoryAsync(int categoryId);
        Task<IEnumerable<Car>> GetCarsByLocationAsync(int locationId);
        Task<IEnumerable<Car>> SearchCarsAsync(string? brand, string? model, int? categoryId, int? locationId);

        //Create,Update,Delete
        Task<Car> CreateCarAsync(CarViewModel model);
        Task<Car> UpdateCarAsync(int id, CarViewModel model);
        Task<bool> DeleteCarAsync(int id);
        //Toggle availability
        Task<bool> ToggleCarAvailabilityAsync(int id);
        //feature
        Task<bool> UpdateCarFeaturesAsync(int carId, List<int> FeatureIds);
        // Calculate Price
        Task<decimal> CalculateRentalPriceAsync(int carId, DateTime pickupDate, DateTime returnDate);
        Task<int> CountAsync();
        Task AddReviewAsync(Review review);
        Task<bool> HasUserBookedCarAsync(int userId, int carId);
        Task<bool> HasUserReviewedCarAsync(int userId, int carId);
        Task<int?> GetCompletedBookingIdAsync(int userId, int carId);
        public  Task<Car> CreateCarForPendingAsync(CarViewModel model);

    }
}
