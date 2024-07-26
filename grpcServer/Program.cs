using grpcServer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add gRPC services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<ToDoService>();
app.MapGet("/", () => "Use a gRPC client to communicate with the gRPC endpoints.");

// Set the URLs and ports for the server to listen on
app.Urls.Add("https://localhost:5001");

app.Run();