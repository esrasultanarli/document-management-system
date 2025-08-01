using DocumentManagementSystem.Models;
using DocumentManagementSystem.ViewModels;

namespace DocumentManagementSystem.Services
{
    public interface IAuthService
    {
        Task<bool> RegisterUserAsync(RegisterViewModel model);
        Task<User?> AuthenticateUserAsync(string username, string password);
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<bool> IsUsernameAvailableAsync(string username);
        Task<bool> IsEmailAvailableAsync(string email);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }
} 