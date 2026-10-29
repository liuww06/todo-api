using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TodoApi;

public class TodoApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TodoApiIntegrationTests(WebApplicationFactory<Program> factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task POST_创建任务后_能查询到()
    {
        // 内存中打真实 HTTP：走完整管道（路由 → 模型绑定 → JSON）
        var response = await _client.PostAsJsonAsync("/todos", new { title = "集成测试" });
        response.EnsureSuccessStatusCode();

        var list = await _client.GetFromJsonAsync<List<TodoItem>>("/todos");
        Assert.NotNull(list);
        Assert.Contains(list, t => t.Title == "集成测试");
    }
}
