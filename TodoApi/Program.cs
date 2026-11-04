using System.Reflection;
using System.Runtime.InteropServices;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// 启动时刻 + 版本号：进程生命周期内只计算一次，别放进每次请求都跑的 lambda 里
var startedAt = DateTimeOffset.UtcNow;
var assembly = typeof(Program).Assembly;
var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
              ?? assembly.GetName().Version?.ToString()
              ?? "unknown";

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

// 健康检查：给人看状态，给机器做探针。字段保持轻量，不要在这里做重活
app.MapGet("/health", () =>
{
    var now = DateTimeOffset.UtcNow;
    return Results.Ok(new
    {
        Status = "OK",
        Timestamp = now.ToString("R"),                       // RFC 1123，UTC 时间
        UptimeSeconds = (long)(now - startedAt).TotalSeconds,
        StartedAt = startedAt.ToString("R"),
        Version = version,
        Environment = app.Environment.EnvironmentName,       // Development / Production
        MachineName = Environment.MachineName,               // 容器 ID 片段，用于确认打到哪个实例
        Framework = RuntimeInformation.FrameworkDescription, // .NET 10.0.x
        OS = RuntimeInformation.OSDescription,
        ProcessorCount = Environment.ProcessorCount,
        TodoCount = todos.GetAll().Count                     // 顺带暴露业务状态
    });
});

app.Run();

public record CreateTodoRequest(string Title);

public partial class Program { }