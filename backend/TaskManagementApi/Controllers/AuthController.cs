using Microsoft.AspNetCore.Mvc;
using TaskManagementApi.Models;
using TaskManagementApi.Services;

namespace TaskManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var user = await _authService.ValidateUserAsync(request.Username, request.Password);
            
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }
            
            var token = await _authService.GenerateJwtTokenAsync(user);
            
            return Ok(new
            {
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
    
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}

