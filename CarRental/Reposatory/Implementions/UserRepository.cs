using CarRental.Models;
using CarRental.Models.DBContext;
using CarRental.Reposatory.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Reposatory.Implementions
{
    public class UserRepository : Reposatory<User>, IUserRepository
    {
        public UserRepository(CarRentalDbContext context):base(context) { }
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u=>u.Email == email);
        }
        public async Task<IEnumerable<User>> GetUserByRoleAsync(string Role)
        {
            return await _context.Users
                .Where(u => u.Role == Role && u.IsActive).ToArrayAsync();
        }
        public async Task<int> GetTotalUsersCountAsync()
        {
            return await _context.Users.CountAsync();
        }
        public async Task<int> GetActiveUsersCountAsync()
        {
            return await _context.Users.CountAsync(u => u.IsActive);
        }
        public async Task<bool> IsEmailExistsAsync(string email, int? excludeUserId = null)
        {
            var q = _context.Users.Where(u => u.Email == email);
            if (excludeUserId.HasValue)
            {
                q=q.Where(u=>u.UserId != excludeUserId.Value);
            }
            return await q.AnyAsync();
        }
    }
}
