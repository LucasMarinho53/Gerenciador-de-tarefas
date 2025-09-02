using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

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
                HostName = configuration["ConnectionStrings:RabbitMQ"] ?? "localhost",
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

                StartConsuming();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to connect to RabbitMQ: {ex.Message}");
                // For development purposes, we'll continue without RabbitMQ
            }
        }

        private void StartConsuming()
        {
            if (_channel == null)
            {
                Console.WriteLine("RabbitMQ channel is not available for consuming.");
                return;
            }

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($" [x] Received {message}");

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
            Console.WriteLine("RabbitMQ consumer started.");
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