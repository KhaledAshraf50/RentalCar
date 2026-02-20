using CarRental.Models;
using CarRental.Models.DBContext;
using CarRental.Reposatory.Interfaces;
using CarRental.Service.Interfaces;
using CarRental.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Service.Implementation
{
    public class BookingService : Service<Booking>, IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ICarReposatory _carRepository;
        private readonly IReposatory<Location> _locationRepository;
        private readonly IReposatory<User> _userRepository;
        private readonly CarRentalDbContext _context;
        private readonly ILogger<BookingService> _logger;

        public BookingService(
            IBookingRepository bookingRepository,
            ICarReposatory carRepository,
            IReposatory<Location> locationRepository,
            IReposatory<User> userRepository,
            CarRentalDbContext context,
            ILogger<BookingService> logger) : base(bookingRepository)
        {
            _bookingRepository = bookingRepository;
            _carRepository = carRepository;
            _locationRepository = locationRepository;
            _userRepository = userRepository;
            _context = context;
            _logger = logger;
        }

        public async Task<Booking> CreateBookingAsync(BookingViewModel model, int userId)
        {
            // Validate car exists and is available
            var car = await _carRepository.GetByIDAsync(model.CarId);
            if (car == null)
                throw new InvalidOperationException("Car not found");

            if (!car.IsAvailable)
                throw new InvalidOperationException("This car is currently not available for booking");

            if (!car.IsActive || !car.IsApproved)
                throw new InvalidOperationException("This car is not available for booking");

            // Validate dates
            if (model.PickupDate >= model.ReturnDate)
                throw new InvalidOperationException("Return date must be after pickup date");

            if (model.PickupDate < DateTime.Today)
                throw new InvalidOperationException("Pickup date cannot be in the past");

            // Validate locations
            var pickupLocation = await _locationRepository.GetByIDAsync(model.PickupLocationId);
            if (pickupLocation == null || !pickupLocation.IsActive)
                throw new InvalidOperationException("Invalid pickup location");

            var returnLocation = await _locationRepository.GetByIDAsync(model.ReturnLocationId);
            if (returnLocation == null || !returnLocation.IsActive)
                throw new InvalidOperationException("Invalid return location");

            // Check car availability for these dates
            var isAvailable = await IsCarAvailableForDatesAsync(model.CarId, model.PickupDate, model.ReturnDate);
            if (!isAvailable)
                throw new InvalidOperationException("Car is already booked for the selected dates");

            // Calculate total price
            var totalPrice = await CalculateTotalPriceAsync(model.CarId, model.PickupDate, model.ReturnDate);

            // Create booking
            var booking = new Booking
            {
                UserId = userId,
                CarId = model.CarId,
                PickupLocationId = model.PickupLocationId,
                ReturnLocationId = model.ReturnLocationId,
                PickupDate = model.PickupDate,
                ReturnDate = model.ReturnDate,
                TotalPrice = totalPrice,
                Notes = model.Notes,
                Status = "Pending",
                BookingDate = DateTime.UtcNow,
                BookingReference = GenerateBookingReference()
            };

            await _bookingRepository.AddAsync(booking);

            _logger.LogInformation("Booking created successfully. Reference: {Reference}, User: {UserId}, Car: {CarId}",
                booking.BookingReference, userId, model.CarId);

            return booking;
        }

        public async Task<decimal> CalculateTotalPriceAsync(int carId, DateTime pickupDate, DateTime returnDate)
        {
            var car = await _carRepository.GetByIDAsync(carId);
            if (car == null)
                throw new InvalidOperationException("Car not found");

            var days = (returnDate - pickupDate).Days;
            if (days < 1) days = 1;

            // Apply discounts for longer rentals
            decimal discount = 0;
            if (days >= 30)
                discount = 0.20m; // 20% off for monthly
            else if (days >= 14)
                discount = 0.15m; // 15% off for 2 weeks
            else if (days >= 7)
                discount = 0.10m; // 10% off for weekly

            var subtotal = car.DailyPrice * days;
            var discountAmount = subtotal * discount;

            return subtotal - discountAmount;
        }

        public async Task<bool> IsCarAvailableForDatesAsync(int carId, DateTime pickupDate, DateTime returnDate, int? excludeBookingId = null)
        {
            var query = _context.Bookings
                .Where(b => b.CarId == carId &&
                           b.Status != "Cancelled" &&
                           b.Status != "Completed" &&
                           ((b.PickupDate <= pickupDate && b.ReturnDate >= pickupDate) ||
                            (b.PickupDate <= returnDate && b.ReturnDate >= returnDate) ||
                            (pickupDate <= b.PickupDate && returnDate >= b.PickupDate)));

            if (excludeBookingId.HasValue)
            {
                query = query.Where(b => b.BookingId != excludeBookingId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<IEnumerable<Booking>> GetUserBookingsAsync(int userId)
        {
            return await _context.Bookings
                .Include(b => b.Car)
                    .ThenInclude(c => c.Category)
                .Include(b => b.Car)
                    .ThenInclude(c => c.Location)
                .Include(b => b.PickupLocation)
                .Include(b => b.ReturnLocation)
                .Include(b => b.Review)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
        }

        public async Task<Booking?> GetBookingWithDetailsAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.Car)
                    .ThenInclude(c => c.Category)
                .Include(b => b.Car)
                    .ThenInclude(c => c.Location)
                .Include(b => b.PickupLocation)
                .Include(b => b.ReturnLocation)
                .Include(b => b.User)
                .Include(b => b.Review)
                .FirstOrDefaultAsync(b => b.BookingId == id);
        }

        public async Task<bool> CancelBookingAsync(int bookingId, int userId, string? reason = null)
        {
            var booking = await _bookingRepository.GetByIDAsync(bookingId);
            if (booking == null || booking.UserId != userId)
                return false;

            if (booking.Status == "Completed" || booking.Status == "Cancelled")
                return false;

            booking.Status = "Cancelled";
            booking.CancelledAt = DateTime.UtcNow;
            booking.CancellationReason = reason;

            _bookingRepository.Delete(booking);

            _logger.LogInformation("Booking cancelled. Reference: {Reference}, User: {UserId}",
                booking.BookingReference, userId);

            return true;
        }

        public async Task<bool> UpdateBookingStatusAsync(int bookingId, string status)
        {
            var booking = await _bookingRepository.GetByIDAsync(bookingId);
            if (booking == null) return false;

            var oldStatus = booking.Status;
            booking.Status = status;

            switch (status)
            {
                case "Confirmed":
                    booking.ConfirmedAt = DateTime.UtcNow;
                    break;
                case "Completed":
                    booking.CompletedAt = DateTime.UtcNow;
                    break;
                case "Cancelled":
                    booking.CancelledAt = DateTime.UtcNow;
                    break;
            }

            _bookingRepository.Update(booking);

            _logger.LogInformation("Booking status updated. Reference: {Reference}, Old: {OldStatus}, New: {NewStatus}",
                booking.BookingReference, oldStatus, status);

            return true;
        }

        public async Task<IEnumerable<Booking>> GetAllBookingsAsync()
        {
            return await _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.User)
                .Include(b => b.PickupLocation)
                .Include(b => b.ReturnLocation)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetRecentBookingsAsync(int count = 10)
        {
            return await _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.User)
                .OrderByDescending(b => b.BookingDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<int> GetTotalBookingsCountAsync()
        {
            return await _context.Bookings.CountAsync();
        }

        public async Task<int> GetPendingBookingsCountAsync()
        {
            return await _context.Bookings.CountAsync(b => b.Status == "Pending");
        }

        public async Task<int> GetConfirmedBookingsCountAsync()
        {
            return await _context.Bookings.CountAsync(b => b.Status == "Confirmed");
        }

        public async Task<int> GetCompletedBookingsCountAsync()
        {
            return await _context.Bookings.CountAsync(b => b.Status == "Completed");
        }

        public async Task<int> GetCancelledBookingsCountAsync()
        {
            return await _context.Bookings.CountAsync(b => b.Status == "Cancelled");
        }

        public async Task<decimal> GetTodayRevenueAsync()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            return await _context.Bookings
                .Where(b => b.BookingDate >= today &&
                           b.BookingDate < tomorrow &&
                           (b.Status == "Confirmed" || b.Status == "Completed"))
                .SumAsync(b => b.TotalPrice);
        }

        public async Task<decimal> GetMonthlyRevenueAsync(int? month = null, int? year = null)
        {
            var targetMonth = month ?? DateTime.Now.Month;
            var targetYear = year ?? DateTime.Now.Year;

            var startDate = new DateTime(targetYear, targetMonth, 1);
            var endDate = startDate.AddMonths(1);

            return await _context.Bookings
                .Where(b => b.BookingDate >= startDate &&
                           b.BookingDate < endDate &&
                           (b.Status == "Confirmed" || b.Status == "Completed"))
                .SumAsync(b => b.TotalPrice);
        }

        public async Task<Dictionary<string, int>> GetBookingsByStatusAsync()
        {
            var bookings = await _context.Bookings.ToListAsync();
            return bookings
                .GroupBy(b => b.Status)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public async Task<List<MonthlyRevenueVM>> GetMonthlyRevenueForYearAsync(int year)
        {
            var revenue = new List<MonthlyRevenueVM>();

            for (int month = 1; month <= 12; month++)
            {
                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1);

                var total = await _context.Bookings
                    .Where(b => b.BookingDate >= startDate &&
                               b.BookingDate < endDate &&
                               (b.Status == "Confirmed" || b.Status == "Completed"))
                    .SumAsync(b => b.TotalPrice);

                revenue.Add(new MonthlyRevenueVM
                {
                    Month = month,
                    MonthName = startDate.ToString("MMMM"),
                    Revenue = total
                });
            }

            return revenue;
        }

        private string GenerateBookingReference()
        {
            return $"CAR-{DateTime.Now:yyyyMMdd}-{DateTime.Now:HHmmss}-{new Random().Next(1000, 9999)}";
        }
    }
}