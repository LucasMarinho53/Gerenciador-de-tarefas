using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Data;
using TaskManagementApi.Models;
using TaskManagementApi.Services;

namespace TaskManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly TaskManagementContext _context;
        private readonly IAuthService _authService;
        
        public UsersController(TaskManagementContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new { u.Id, u.Username, u.Email, u.CreatedAt })
                .ToListAsync();
            
            return Ok(users);
        }
        
        [HttpPost("createRandom")]
        public async Task<IActionResult> CreateRandomUsers([FromBody] CreateRandomUsersRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var users = new List<User>();
            var random = new Random();
            
            for (int i = 0; i < request.Amount; i++)
            {
                var randomPart = random.Next(100000, 999999).ToString();
                var username = request.UserNameMask.Replace("{{random}}", randomPart);
                var email = $"{username}@example.com";
                var password = _authService.HashPassword("password123"); // Default password
                
                var user = new User
                {
                    Username = username,
                    Email = email,
                    Password = password
                };
                
                users.Add(user);
            }
            
            try
            {
                _context.Users.AddRange(users);
                await _context.SaveChangesAsync();
                
                return Ok(new
                {
                    message = $"{request.Amount} users created successfully",
                    users = users.Select(u => new { u.Id, u.Username, u.Email }).ToList()
                });
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(new { message = "Error creating users. Some usernames might already exist.", error = ex.Message });
            }
        }
        
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            // Check if user already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Email);
            
            if (existingUser != null)
            {
                return BadRequest(new { message = "Username or email already exists" });
            }
            
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                Password = _authService.HashPassword(request.Password)
            };
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            var token = await _authService.GenerateJwtTokenAsync(user);
            
            return Ok(new
            {
                message = "User registered successfully",
                token,
                user = new
                {
                    id = user.Id,
                    username = user.Username,
                    email = user.Email
                }
            });
        }
    }
    
    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}

