using CarRental.Models;

namespace CarRental.Reposatory.Interfaces
{
    public interface IReviewRepository : IReposatory<Review>
    {
        Task<IEnumerable<ReviewAdminVM>> GetReviewsForAdminAsync(string? filter,string? search);
        Task ApproveReviewAsync(int id, int adminId);
        Task RejectReviewAsync(int id);
        Task AddReviewAsync(Review review);
        Task<bool> HasUserBookedCarAsync(int userId, int carId);
        Task<bool> HasUserReviewedCarAsync(int userId, int carId);
        Task<int?> GetCompletedBookingIdAsync(int userId, int carId);

    }
}
