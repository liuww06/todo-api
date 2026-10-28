namespace TodoApi.Tests;

public class TodoServiceTests
{
    [Fact]
    public void Add_有效标题_返回未完成的新任务()
    {
        // Arrange
        var service = new TodoService();
        var title = "学习 CI/CD";

        // Act
        var item = service.Add(title);

        // Assert
        Assert.NotNull(item);
        Assert.Equal(title, item.Title);
        Assert.False(item.IsDone);
        Assert.Equal(1, item.Id);
    }

    [Fact]
    public void Add_空白标题_应抛出异常()
    {
        // Arrange
        var service = new TodoService();
        var title = "";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => service.Add(title));
    }
    
    [Fact]
    public void Complete_存在的任务_标记为完成()
    {
        var service = new TodoService();
        var item = service.Add("写 Dockerfile");

        var result = service.Complete(item.Id);

        Assert.False(result);
        Assert.Contains(service.GetAll(), t => t.Id == item.Id && t.IsDone);
    }

    [Fact]
    public void Complete_不存在的任务_返回false()
    {
        var service = new TodoService();

        Assert.False(service.Complete(999));
    }
}
