using TodoApi;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var todos = new TodoService();

// 列表所有任务
app.MapGet("/todos", () => todos.GetAll());

// 新建任务
app.MapPost("/todos", (CreateTodoRequest request) =>
{
    var item = todos.Add(request.Title);
    return Results.Created($"/todos/{item.Id}", item);
});


// 完成任务
app.MapPut("/todos/{id}/complete", (int id) =>
    todos.Complete(id) ? Results.NoContent() : Results.NotFound());

app.Run();

public record CreateTodoRequest(string Title);
