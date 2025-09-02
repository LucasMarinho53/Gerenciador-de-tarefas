using System.ComponentModel.DataAnnotations;

namespace TaskManagementApi.Models
{
    public class TaskItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
        
        public DateTime DueDate { get; set; }
        
        public TaskStatus Status { get; set; } = TaskStatus.Pending;
        
        public Guid AssignedUserId { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
    
    public enum TaskStatus
    {
        Pending = 0,
        InProgress = 1,
        Completed = 2
    }
}

