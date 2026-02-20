using CarRental.Models;
using CarRental.ViewModels;

namespace CarRental.Service.Interfaces
{
    public interface IBookingService : IService<Booking>
    {
        Task<Booking> CreateBookingAsync(BookingViewModel model, int userId);
        Task<IEnumerable<Booking>> GetUserBookingsAsync(int userId);
        Task<Booking?> GetBookingWithDetailsAsync(int id);
        Task<bool> CancelBookingAsync(int bookingId, int userId, string? reason = null);
        Task<bool> UpdateBookingStatusAsync(int bookingId, string status);
        Task<bool> IsCarAvailableForDatesAsync(int carId, DateTime pickupDate, DateTime returnDate, int? excludeBookingId = null);
        Task<decimal> CalculateTotalPriceAsync(int carId, DateTime pickupDate, DateTime returnDate);

        // Admin Methods
        Task<IEnumerable<Booking>> GetAllBookingsAsync();
        Task<IEnumerable<Booking>> GetRecentBookingsAsync(int count = 10);
        Task<int> GetTotalBookingsCountAsync();
        Task<int> GetPendingBookingsCountAsync();
        Task<int> GetConfirmedBookingsCountAsync();
        Task<int> GetCompletedBookingsCountAsync();
        Task<int> GetCancelledBookingsCountAsync();
        Task<decimal> GetTodayRevenueAsync();
        Task<decimal> GetMonthlyRevenueAsync(int? month = null, int? year = null);
        Task<Dictionary<string, int>> GetBookingsByStatusAsync();
        Task<List<MonthlyRevenueVM>> GetMonthlyRevenueForYearAsync(int year);
    }
}