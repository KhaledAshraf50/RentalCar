using BCrypt.Net;
using CarRental.Models;
using CarRental.Reposatory.Interfaces;
using CarRental.Service.Interfaces;
using CarRental.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Service.Implementation
{
    public class UserService : Service<User>, IUserService
    {
        readonly IUserRepository _userRepository;
        readonly IWebHostEnvironment _webHostEnvironment;
        public UserService(IUserRepository userRepo, IWebHostEnvironment webHostEnvironment):base(userRepo)
        {
            _userRepository = userRepo;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<bool> ValidateUserAsync(string email, string password)
        {
            var user = await GetUserByEmailAsync(email);
            if (user == null || !user.IsActive) return false;
            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            //user.PasswordHash == password;    xxxxxxxxx
            //123456          xxxx
            //$2a$11$Xk3....  Hash
        }
        public async Task<User> RegisterUserAsync(UserViewModel model)
        {
            // Check if email already exists
            if (await _userRepository.IsEmailExistsAsync(model.Email))
                throw new InvalidOperationException("Email Already Registered");
            // Handle Profile image Upload
            string profileImageUrl = await UploadProfileImageAsync(model.ProfileImageFile);
            // create user 
            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                Phone = model.Phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword),
                Role = model.Role,
                DateOfBirth = model.DateOfBirth,
                Address = model.Address,
                ProfileImage = profileImageUrl,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            };
            await _userRepository.AddAsync(user);
            return user;
        }
        public async Task<bool> ChangeUserRoleAsync(int userId, string role)
        {
            var user = await _userRepository.GetByIDAsync(userId);
            if (user == null) return false;

            user.Role = role;
             _userRepository.Update(user);
            return true;
        }
        public async Task<bool> UpdateUserProfileAsync(int userId, UserViewModel model)
        {
            var user = await _userRepository.GetByIDAsync(userId);
            if(user == null) return false;
            // check if email already exists (excludeing current user)
            if (await _userRepository.IsEmailExistsAsync(model.Email, userId))
                throw new InvalidOperationException("Email already registered by another User");
            // Update User Properties
            user.FullName = model.FullName;
            user.Email = model.Email;
            user.Phone = model.Phone;
            user.DateOfBirth = model.DateOfBirth;
            user.Address = model.Address;
            //user.Role = model.Role; xxxxxxxxxxxxxx
            // Handle Profile Image Update 
            if(model.ProfileImageFile != null)
            {
                user.ProfileImage = await UploadProfileImageAsync(model.ProfileImageFile);
            }
            _userRepository.Update(user);
            return true;
        }
        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByIDAsync(userId);
            if(user == null || !BCrypt.Net.BCrypt.Verify(currentPassword,user.PasswordHash))
                return false;
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _userRepository.Update(user);
            return true;
        }
        public async Task<bool> ToggleUserStatusAsync(int userId)
        {
            var user = await _userRepository.GetByIDAsync(userId);
            if(user == null) return false;
            user.IsActive = !user.IsActive;
            _userRepository.Update(user);
            return true;
        }
        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
        {
            return await _userRepository.GetUserByRoleAsync(role);
        }
        public async Task<int> GetTotalUsersCountAsync()
        {
            return await _userRepository.GetTotalUsersCountAsync();
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetUserByEmailAsync(email);
        }
       

        public async Task<int> GetActiveUsersCountAsync()
        {
            return await _userRepository.GetActiveUsersCountAsync();
        }
     
        async Task<string> UploadProfileImageAsync(IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return "/imgs/profile_img.svg";//Default Profile Image
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "profiles");
            if(!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using(var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }
            return $"/uploads/profiles/{uniqueFileName}";
        }
    }
}
