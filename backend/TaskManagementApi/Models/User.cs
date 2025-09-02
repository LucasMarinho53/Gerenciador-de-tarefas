using System.ComponentModel.DataAnnotations;

namespace TaskManagementApi.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Password { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public List<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
    }
}

