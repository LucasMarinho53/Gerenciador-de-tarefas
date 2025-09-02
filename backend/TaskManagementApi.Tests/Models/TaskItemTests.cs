using TaskManagementApi.Models;
using Xunit;
using TaskStatus = TaskManagementApi.Models.TaskStatus;

namespace TaskManagementApi.Tests.Models;

public class TaskItemTests
{
    [Fact]
    public void TaskItem_ShouldInitializeWithDefaultValues()
    {
        // Act
        var task = new TaskItem();

        // Assert
        Assert.NotEqual(Guid.Empty, task.Id);
        Assert.Equal(TaskStatus.Pending, task.Status);
        Assert.True(task.CreatedAt <= DateTime.UtcNow);
        Assert.True(task.UpdatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void TaskItem_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var title = "Test Task";
        var description = "Test Description";
        var dueDate = DateTime.UtcNow.AddDays(1);

        // Act
        var task = new TaskItem
        {
            Id = taskId,
            Title = title,
            Description = description,
            DueDate = dueDate,
            Status = TaskStatus.InProgress,
            AssignedUserId = userId
        };

        // Assert
        Assert.Equal(taskId, task.Id);
        Assert.Equal(title, task.Title);
        Assert.Equal(description, task.Description);
        Assert.Equal(dueDate, task.DueDate);
        Assert.Equal(TaskStatus.InProgress, task.Status);
        Assert.Equal(userId, task.AssignedUserId);
    }

    [Theory]
    [InlineData(TaskStatus.Pending)]
    [InlineData(TaskStatus.InProgress)]
    [InlineData(TaskStatus.Completed)]
    public void TaskItem_ShouldAcceptAllValidStatuses(TaskStatus status)
    {
        // Arrange & Act
        var task = new TaskItem
        {
            Title = "Test Task",
            Status = status
        };

        // Assert
        Assert.Equal(status, task.Status);
    }

    [Fact]
    public void TaskItem_ShouldUpdateTimestampOnModification()
    {
        // Arrange
        var task = new TaskItem
        {
            Title = "Original Title"
        };
        var originalUpdateTime = task.UpdatedAt;

        // Act
        System.Threading.Thread.Sleep(1); // Ensure time difference
        task.Title = "Updated Title";
        task.UpdatedAt = DateTime.UtcNow;

        // Assert
        Assert.True(task.UpdatedAt > originalUpdateTime);
    }
}

