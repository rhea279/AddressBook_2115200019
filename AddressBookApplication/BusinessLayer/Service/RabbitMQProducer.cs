using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using Microsoft.Extensions.Configuration;
using BusinessLayer.Interface;

namespace BusinessLayer.Service
{
    public class RabbitMQProducer : IRabbitMQProducer
    {
        private readonly string _host;
        private readonly string _exchangeName;
        private readonly string _queueName;
        private readonly string _routingKey;

        public RabbitMQProducer(IConfiguration configuration)
        {
            _host = configuration["RabbitMQ:Host"];
            _exchangeName = configuration["RabbitMQ:ExchangeName"];  // Use an exchange
            _queueName = configuration["RabbitMQ:QueueName"];
            _routingKey = configuration["RabbitMQ:RoutingKey"];  // Ensure routing key is set
        }

        public void PublishMessage<T>(T message)
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = _host,
                    UserName = "guest",
                    Password = "guest"
                };

                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();

                // Log queue name
                Console.WriteLine($"Publishing to queue: {_queueName}");

                // Declare queue again (ensures it exists)
                channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                var json = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(json);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                // Log message
                Console.WriteLine($"Sending message: {json}");

                channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: properties, body: body);
                Console.WriteLine($"[x] Sent: {json}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[!] Error publishing message: {ex.Message}");
            }
        }

    
}
}
