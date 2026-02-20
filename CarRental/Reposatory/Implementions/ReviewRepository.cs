using CarRental.Models;
using CarRental.Models.DBContext;
using CarRental.Reposatory.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Reposatory.Implementions
{
    public class ReviewRepository : Reposatory<Review>, IReviewRepository
    {
        private readonly CarRentalDbContext _context;
        public ReviewRepository(CarRentalDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<ReviewAdminVM>> GetReviewsForAdminAsync(string? filter, string? search)
        {
            var query= _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Car)
                .AsQueryable();
            if (!string.IsNullOrEmpty(filter))
            {
                if (filter == "pending")
                    query = query.Where(r => !r.IsApproved);

                else if (filter == "approved")
                    query = query.Where(r => r.IsApproved);
            }
            //search
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(r =>
                r.User.FullName.ToLower().Contains(search) ||
                r.Car.Brand.ToLower().Contains(search) ||
                r.Car.Model.ToLower().Contains(search) ||
                r.Comment!.ToLower().Contains(search)
                    );
            }
            return await query
                .Select(r => new ReviewAdminVM
                {
                    ReviewId = r.ReviewId,
                    UserName = r.User.FullName,
                    ImageUrl = r.User.ProfileImage,
                    CarName = r.Car.Brand + " " + r.Car.Model,
                    ReviewDate = r.ReviewDate,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    IsApproved = r.IsApproved
                })
                .ToListAsync();

        }
        public async Task ApproveReviewAsync(int id, int adminId)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                review.IsApproved = true;
                review.ApprovedAt = DateTime.UtcNow;
                review.ApprovedByUserId = adminId;
                _context.Update(review);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RejectReviewAsync(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddReviewAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasUserBookedCarAsync(int userId, int carId)
        {
            return await _context.Bookings
                .AnyAsync(b =>
                b.UserId == userId &&
                b.CarId == carId &&
                b.Status == "Confirmed");
        }

        public async Task<bool> HasUserReviewedCarAsync(int userId, int carId)
        {
            return await _context.Reviews
                .AnyAsync(b =>
                b.UserId == userId &&
                b.CarId == carId);
        }

        public async Task<int?> GetCompletedBookingIdAsync(int userId, int carId)
        {
            var booking = await _context.Bookings
                .Where(b =>
                b.UserId == userId &&
                b.CarId == carId &&
                b.Status == "Confirmed")
                .OrderByDescending(b => b.ReturnDate)
                .FirstOrDefaultAsync();
            return booking?.BookingId;
        }
    }
}
