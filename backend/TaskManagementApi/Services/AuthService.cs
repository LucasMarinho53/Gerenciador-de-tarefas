using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TaskManagementApi.Data;
using TaskManagementApi.Models;

namespace TaskManagementApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly TaskManagementContext _context;
        private readonly IConfiguration _configuration;
        
        public AuthService(TaskManagementContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        
        public async Task<string> GenerateJwtTokenAsync(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "your-super-secret-key-that-is-at-least-32-characters-long");
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"] ?? "TaskManagementApi",
                Audience = _configuration["Jwt:Audience"] ?? "TaskManagementApi"
            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        
        public async Task<User?> ValidateUserAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            
            if (user == null || !VerifyPassword(password, user.Password))
            {
                return null;
            }
            
            return user;
        }
        
        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "salt"));
            return Convert.ToBase64String(hashedBytes);
        }
        
        public bool VerifyPassword(string password, string hashedPassword)
        {
            var hashedInput = HashPassword(password);
            return hashedInput == hashedPassword;
        }
    }
}

