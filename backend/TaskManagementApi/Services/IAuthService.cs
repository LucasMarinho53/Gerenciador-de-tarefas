using TaskManagementApi.Models;

namespace TaskManagementApi.Services
{
    public interface IAuthService
    {
        Task<string> GenerateJwtTokenAsync(User user);
        Task<User?> ValidateUserAsync(string username, string password);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }
}

