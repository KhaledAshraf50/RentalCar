using CarRental.Models;
using CarRental.ViewModels;

namespace CarRental.Service.Interfaces
{
    public interface IUserService:IService<User>
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> ValidateUserAsync(string email,string  password);
        Task<User> RegisterUserAsync(UserViewModel model);
        Task<bool> UpdateUserProfileAsync(int  userId, UserViewModel model);
        Task<bool> ChangePasswordAsync(int userId,string currentPassword,string newPassword);
        Task<IEnumerable<User>> GetUsersByRoleAsync(string role);
        Task<bool> ToggleUserStatusAsync(int userId);
        Task<int> GetTotalUsersCountAsync();
        Task<int> GetActiveUsersCountAsync();
        Task<bool> ChangeUserRoleAsync(int userId, string role);
    }
}
