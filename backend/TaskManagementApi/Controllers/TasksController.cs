using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagementApi.Data;
using TaskManagementApi.Models;
using TaskManagementApi.Services;

namespace TaskManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly TaskManagementContext _context;
        private readonly IRabbitMQService _rabbitMQService;
        
        public TasksController(TaskManagementContext context, IRabbitMQService rabbitMQService)
        {
            _context = context;
            _rabbitMQService = rabbitMQService;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks()
        {
            var userId = GetCurrentUserId();
            var tasks = await _context.Tasks
                .Where(t => t.AssignedUserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
            
            return Ok(tasks);
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> GetTask(Guid id)
        {
            var userId = GetCurrentUserId();
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id && t.AssignedUserId == userId);
            
            if (task == null)
            {
                return NotFound();
            }
            
            return Ok(task);
        }
        
        [HttpPost]
        public async Task<ActionResult<TaskItem>> CreateTask(CreateTaskRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var userId = GetCurrentUserId();
            
            var task = new TaskItem
            {
                Title = request.Title,
                Description = request.Description,
                DueDate = request.DueDate,
                AssignedUserId = userId,
                Status = TaskManagementApi.Models.TaskStatus.Pending
            };
            
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            
            // Send notification
            await _rabbitMQService.PublishTaskAssignmentNotificationAsync(userId, task.Id, task.Title);
            
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(Guid id, UpdateTaskRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var userId = GetCurrentUserId();
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id && t.AssignedUserId == userId);
            
            if (task == null)
            {
                return NotFound();
            }
            
            task.Title = request.Title;
            task.Description = request.Description;
            task.DueDate = request.DueDate;
            task.Status = request.Status;
            task.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            
            return Ok(task);
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var userId = GetCurrentUserId();
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id && t.AssignedUserId == userId);
            
            if (task == null)
            {
                return NotFound();
            }
            
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }
        
        [HttpPost("{id}/assign/{assigneeId}")]
        public async Task<IActionResult> AssignTask(Guid id, Guid assigneeId)
        {
            var userId = GetCurrentUserId();
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id && t.AssignedUserId == userId);
            
            if (task == null)
            {
                return NotFound("Task not found");
            }
            
            var assignee = await _context.Users.FindAsync(assigneeId);
            if (assignee == null)
            {
                return NotFound("Assignee not found");
            }
            
            task.AssignedUserId = assigneeId;
            task.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            
            // Send notification to the new assignee
            await _rabbitMQService.PublishTaskAssignmentNotificationAsync(assigneeId, task.Id, task.Title);
            
            return Ok(task);
        }
        
        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException());
        }
    }
    
    public class CreateTaskRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
    }
    
    public class UpdateTaskRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public TaskManagementApi.Models.TaskStatus Status { get; set; }
    }
}

