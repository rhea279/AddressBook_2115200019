using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Middleware.RabbitMQ
{
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly string _host;
        private readonly string _exchangeName;
        private readonly string _queueName;
        private readonly string _routingKey;

        public RabbitMQConsumer(IConfiguration configuration)
        {
            _host = configuration["RabbitMQ:Host"];
            _exchangeName = configuration["RabbitMQ:ExchangeName"];
            _queueName = configuration["RabbitMQ:QueueName"];
            _routingKey = configuration["RabbitMQ:RoutingKey"];
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = _host };
                var connection = factory.CreateConnection();
                var channel = connection.CreateModel();

                // Ensure queue exists
                channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                Console.WriteLine($"Listening to queue: {_queueName}");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"[x] Received: {message}");

                    // Acknowledge message
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };

                channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[!] Consumer Error: {ex.Message}");
            }

            return Task.CompletedTask;
        }

    }
}
