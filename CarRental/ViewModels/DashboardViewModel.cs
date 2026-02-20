using CarRental.Models;

namespace CarRental.ViewModels
{
    public class DashboardViewModel
    {
        // Admin Dashboard
        public int TotalCars { get; set; }
        public int TotalBookings { get; set; }
        public int PendingBookings { get; set; }
        public int CompletedBookings { get; set; }
        public int TotalUsers { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public IEnumerable<Booking>? RecentBookings { get; set; } 

        // User Dashboard
        public int ActiveBookings { get; set; }
        public int CompletedTrips { get; set; }
        public decimal TotalSpent { get; set; }
        public List<Booking> UserRecentBookings { get; set; } = new();
    }
}