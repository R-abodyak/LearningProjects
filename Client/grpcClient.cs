using Grpc.Core;
using Grpc.Net.Client;
using GrpcTodo;

namespace grpcClient;

public class GrpcClient
{
    private readonly TodoService.TodoServiceClient _client;
    public GrpcClient()
    {
        using var channel = GrpcChannel.ForAddress("https://localhost:5001");
        _client = new TodoService.TodoServiceClient(channel);
    }
    public async Task RunAsync()
    {
        // Add a new To do
        var addTodoResponse = await _client.AddTodoAsync(new AddTodoRequest { Title = "Task 1", Description = "Description 1", DueDate = "2024-12-31" });
        Console.WriteLine($"Added Todo: {addTodoResponse.Item.Id}, {addTodoResponse.Item.Title}");
        
        var addTodoResponse2 = await _client.AddTodoAsync(new AddTodoRequest { Title = "Task 2", Description = "Description 2", DueDate = "2024-12-31" });
        Console.WriteLine($"Added Todo: {addTodoResponse2.Item.Id}, {addTodoResponse.Item.Title}");

        var updateTodoResponse = await _client.UpdateTodoAsync(new item { Id = 1, Title = "Task 1 Updated", Description = "Description 1 Updated", DueDate = "2024-12-31", IsCompleted = true });
        
        
        // Get all Todos
        using var call = _client.GetTodos(new GetTodosRequest());
        await foreach (var response in call.ResponseStream.ReadAllAsync())
        {
            foreach (var item in response.Items)
            {
                Console.WriteLine($"Todo: {item.Id}, {item.Title}");
            }
        }
    }
}