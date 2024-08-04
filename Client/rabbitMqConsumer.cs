using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;


namespace grpcClient;

public class RabbitMqConsumer
{
    private readonly IModel _channel;
    private readonly EventingBasicConsumer _consumer;

    public RabbitMqConsumer()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();
        _channel.QueueDeclare(queue: "todo-queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

        _consumer = new EventingBasicConsumer(_channel);
        _consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine(" [x] Received {0}", message);
            // Process the message (you can add your custom logic here)
        };
    }

    public void Start()
    {
        _channel.BasicConsume(queue: "todo-queue", autoAck: true, consumer: _consumer);
        Console.WriteLine(" [*] Waiting for messages.");
    }
}