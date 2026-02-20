using CarRental.Models;
using CarRental.Models.DBContext;
using CarRental.Reposatory.Interfaces;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CarRental.Reposatory.Implementions
{
    public class BookingRepository : Reposatory<Booking>, IBookingRepository
    {
        public BookingRepository(CarRentalDbContext context):base(context) { }
        public async Task<IEnumerable<Booking>> GetUserBookingsAsync(int userId)
        {
            return await _context.Bookings.
                Include(b=>b.Car)
                .Include(b=>b.PickupLocation)
                .Include(b=>b.ReturnLocation)
                .Where(b=>b.UserId == userId)
                .OrderByDescending(b=>b.BookingDate)
                .ToListAsync();
        }
        public async Task<IEnumerable<Booking>> GetRecentBookingsAsync(int count = 10)
        {
            return await _context.Bookings
                .Include(b=>b.Car)
                .Include(b=>b.User)
                .OrderByDescending(b=>b.BookingDate)
                .Take(count)
                .ToListAsync();
        }
        public async Task<Booking?> GetBookingWithDetailsAsync(int id)
        {
            return await _context.Bookings
                .Include(b=>b.Car)
                .Include(b=>b.PickupLocation)
                .Include(b=>b.ReturnLocation)
                .Include(b=>b.User)
                .Include(b=>b.Review)
                .FirstOrDefaultAsync(b=>b.BookingId == id);
        }
        public async Task<int> GetTotalBookingsCountAsync()
        {
            return await _context.Bookings.CountAsync();
        }
        public async Task<int> GetPendingBookingsCountAsync()
        {
            return await _context.Bookings.CountAsync(b=>b.Status=="Pending");
        }
        public async Task<int> GetCompleteBookingsCountAsync()
        {
            return await _context.Bookings.CountAsync(b => b.Status == "Completed");
        }

        public async Task<decimal> GetMonthlyRevenueAsync(int month, int year)
        {
            var startDate = new DateTime(year,month,1);
            var endDate = startDate.AddMonths(1);
            return await _context.Bookings
                .Where(b => b.BookingDate >= startDate
                && b.BookingDate < endDate && (b.Status == "Confirmed" || b.Status == "Completed"))
                .SumAsync(b=>b.TotalPrice);
        }
        public async Task<bool> IsCarAvailableForDatesAsync(int carId, DateTime PickupDate, DateTime returnDate, int? excludeBookingId = null)
        {
            var q = _context.Bookings.Where(b => b.CarId == carId && b.Status != "Cancelled" &&
            ((b.PickupDate <= PickupDate && b.ReturnDate >= PickupDate) ||
            (b.PickupDate <= returnDate && b.ReturnDate >= returnDate) ||
            (PickupDate <= b.PickupDate && returnDate >= b.PickupDate))
            );
            if (excludeBookingId.HasValue)
            {
                q = q.Where(b => b.BookingId != excludeBookingId.Value);
            }
            return !await q.AnyAsync();
        }
    }
}
