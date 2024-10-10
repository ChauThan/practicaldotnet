using System.Text.Json;
using TodoApp.Console.Models;

namespace TodoApp.Console.Data;

public class DataContext
{
    private readonly string _filePath;

    private Data _data;

    public DataContext()
    {
        _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.json");
        _data = LoadData();
    }

    public ICollection<TodoItem> TodoItems => _data.TodoItems;
    public ICollection<TodoList> TodoLists => _data.TodoLists;

    public virtual async Task SaveChangesAsync()
    {
        using var fs = new FileStream(_filePath, FileMode.OpenOrCreate);
        await JsonSerializer.SerializeAsync(fs, _data);
    }

    private Data LoadData()
    {
        if (!File.Exists(_filePath))
        {
            return new Data([], []);
        }

        using var fs = new FileStream(_filePath, FileMode.Open);
        return
            JsonSerializer.Deserialize<Data>(fs)
            ?? new Data([], []);
    }

    public record Data(
        ICollection<TodoItem> TodoItems,
        ICollection<TodoList> TodoLists);
}
