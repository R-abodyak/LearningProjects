using System.Text;
using Grpc.Core;
using GrpcTodo;
using RabbitMQ.Client;

namespace grpcServer.Services;

public class ToDoService : TodoService.TodoServiceBase
{
    private static readonly List<item> TodoItems = [];
    private static int _currentId = 1;
    private readonly IModel _channel;

    
    public ToDoService()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();
        _channel.QueueDeclare(queue: "todo-queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
    }
    
    private void PublishMessage(string action, item item)
    {
        var message = $"{action}: {item.Title} (ID: {item.Id})";
        var body = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(exchange: "", routingKey: "todo-queue", basicProperties: null, body: body);
    }
    
    public override Task<AddTodoResponse> AddTodo (AddTodoRequest request, ServerCallContext context)
    {
        var item = new item()
        {
            Id = _currentId++,
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate,
            IsCompleted = false
        };
        TodoItems.Add(item);
        
        // Publish the message to RabbitMQ
        PublishMessage("Added", item);
        return Task.FromResult(new AddTodoResponse{ Item = item });
    }
    public override async Task GetTodos(GetTodosRequest request, IServerStreamWriter<GetTodosResponse> responseStream, ServerCallContext context)
    {
        foreach (var todoItem in TodoItems)
        {
            var response = new GetTodosResponse();
            response.Items.Add(todoItem);
            await responseStream.WriteAsync(response);
        }
    }
    
    public override Task<item> UpdateTodo(item request, ServerCallContext context)
    {
        var item = TodoItems.FirstOrDefault(i => i.Id == request.Id);
        if (item == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Item not found"));
        }

        item.Title = request.Title;
        item.Description = request.Description;
        item.DueDate = request.DueDate;
        item.IsCompleted = request.IsCompleted;
        
        PublishMessage("Updated", item);

        // Return the updated item
        return Task.FromResult(item);
    }
}