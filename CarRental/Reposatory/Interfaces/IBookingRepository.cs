using CarRental.Models;

namespace CarRental.Reposatory.Interfaces
{
    public interface IBookingRepository:IReposatory<Booking>
    {
        Task<IEnumerable<Booking>> GetUserBookingsAsync(int userId);
        Task<IEnumerable<Booking>> GetRecentBookingsAsync(int count = 10);
        Task<Booking?> GetBookingWithDetailsAsync(int id);
        Task<int> GetTotalBookingsCountAsync();
        Task<int> GetPendingBookingsCountAsync();
        Task<int> GetCompleteBookingsCountAsync();
        Task<decimal> GetMonthlyRevenueAsync(int month, int year);
        Task<bool> IsCarAvailableForDatesAsync(int carId, DateTime PickupDate, DateTime returnDate, int? excludeBookingId = null);
    }
}
