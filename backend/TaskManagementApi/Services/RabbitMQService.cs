using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace TaskManagementApi.Services
{
    public class RabbitMQService : IRabbitMQService, IDisposable
    {
        private readonly IConnection? _connection;
        private readonly IModel? _channel;
        private readonly string _queueName = "task_notifications";
        
        public RabbitMQService(IConfiguration configuration)
        {
            var factory = new ConnectionFactory()
            {
                HostName = configuration.GetConnectionString("RabbitMQ") ?? "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };
            
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                
                // Declare the queue
                _channel.QueueDeclare(
                    queue: _queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to connect to RabbitMQ: {ex.Message}");
                // For development purposes, we'll continue without RabbitMQ
            }
        }
        
        public async Task PublishTaskAssignmentNotificationAsync(Guid userId, Guid taskId, string taskTitle)
        {
            try
            {
                if (_channel == null || _channel.IsClosed)
                {
                    Console.WriteLine("RabbitMQ channel is not available. Notification not sent.");
                    return;
                }
                
                var notification = new
                {
                    UserId = userId,
                    TaskId = taskId,
                    TaskTitle = taskTitle,
                    Message = $"Nova tarefa atribuída: {taskTitle}",
                    Timestamp = DateTime.UtcNow
                };
                
                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(notification));
                
                _channel.BasicPublish(
                    exchange: "",
                    routingKey: _queueName,
                    basicProperties: null,
                    body: body);
                
                Console.WriteLine($"Notification sent for task assignment: {taskTitle} to user {userId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to publish notification: {ex.Message}");
            }
            
            await Task.CompletedTask;
        }
        
        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}

