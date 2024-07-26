using Grpc.Core;
using GrpcTodo;

namespace grpcServer.Services;

public class ToDoService : TodoService.TodoServiceBase
{
    private static readonly List<item> TodoItems = new List<item>();
    private static int _currentId = 1;
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
}