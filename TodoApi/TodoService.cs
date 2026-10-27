namespace TodoApi;

public record TodoItem(int Id, string Title, bool IsDone);

public class TodoService
{
    private readonly List<TodoItem> _items = [];

    private int _nextId = 1;

    public TodoItem Add(string title)
    {
        if(string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("标题不能为空", nameof(title));
        }

        var item = new TodoItem(_nextId++, title, false);
        _items.Add(item);
        return item;
    }

    public IReadOnlyList<TodoItem> GetAll() => _items;

    public bool Complete(int id)
    {
        var item = _items.FirstOrDefault(x => x.Id == id);
        if (item == null)
        {
            return false;
        }
        _items.Remove(item);
        _items.Add(new TodoItem(item.Id, item.Title, true));
        return true;
    }
}