using System.Text.RegularExpressions;
using LiteDB;

namespace Console.Todo;

internal class TodoService
{
    private readonly ILiteCollection<TodoItem> _todoCollection;

    public TodoService(string databasePath)
    {
        var db = new LiteDatabase(databasePath);
        _todoCollection = db.GetCollection<TodoItem>("todos");
    }

    public void AddTodo(string text, string listName)
    {
        var todoItem = new TodoItem
        {
            Text = text,
            ListName = string.IsNullOrWhiteSpace(listName) ? "Uncategorized" : listName,
            IsCompleted = false,
            CreatedAt = DateTime.Now
        };
        _todoCollection.Insert(todoItem);
    }

    public IEnumerable<TodoItem> GetTodos() => _todoCollection.FindAll().OrderByDescending(todo => todo.CreatedAt);

    public IEnumerable<TodoItem> GetTodosByList(string pattern)
    {
        var regexPattern = "^" + Regex.Escape(pattern).Replace("\\*", ".*") + "$";
        var regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
        return _todoCollection.FindAll().Where(todo => regex.IsMatch(todo.ListName)).OrderByDescending(todo => todo.CreatedAt);
    }

    public void UpdateTodo(TodoItem todo) => _todoCollection.Update(todo);

    public TodoItem? GetTodoByText(string text) => _todoCollection.FindOne(todo => todo.Text == text);
}