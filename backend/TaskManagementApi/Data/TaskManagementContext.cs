using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Models;

namespace TaskManagementApi.Data
{
    public class TaskManagementContext : DbContext
    {
        public TaskManagementContext(DbContextOptions<TaskManagementContext> options) : base(options)
        {
        }
        
        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<User> Users { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure TaskItem
            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Status).HasConversion<int>();
            });
            
            // Configure User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });
            
            // Configure relationship
            modelBuilder.Entity<TaskItem>()
                .HasOne<User>()
                .WithMany(u => u.AssignedTasks)
                .HasForeignKey(t => t.AssignedUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

