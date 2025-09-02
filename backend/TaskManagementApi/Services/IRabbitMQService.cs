namespace TaskManagementApi.Services
{
    public interface IRabbitMQService
    {
        Task PublishTaskAssignmentNotificationAsync(Guid userId, Guid taskId, string taskTitle);
        void Dispose();
    }
}

