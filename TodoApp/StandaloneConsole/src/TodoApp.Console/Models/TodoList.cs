namespace TodoApp.Console.Models;

public class TodoList
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Title { get; set; } = string.Empty;
}
