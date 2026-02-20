using CarRental.Models;

namespace CarRental.Reposatory.Interfaces
{
    public interface IUserRepository:IReposatory<User>
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<IEnumerable<User>> GetUserByRoleAsync(string Role);
        Task<int> GetTotalUsersCountAsync();
        Task<int> GetActiveUsersCountAsync();
        Task<bool> IsEmailExistsAsync(string email,int? excludeUserId = null);
    }
}
