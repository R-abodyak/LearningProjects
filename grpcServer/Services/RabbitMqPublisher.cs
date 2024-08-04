using RabbitMQ.Client;
using System.Text;
using GrpcTodo;

namespace grpcServer.Services;

public class RabbitMqPublisher
{
    private readonly IModel _channel;

    public RabbitMqPublisher()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();
        _channel.QueueDeclare(queue: "todo-queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    public void PublishMessage(string action, item item)
    {
        var message = $"{action}: {item.Title} (ID: {item.Id})( Description : {item.Description}";
        var body = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(exchange: "", routingKey: "todo-queue", basicProperties: null, body: body);
    }
}