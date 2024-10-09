namespace TodoApp.Console.Models;

public class TodoItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public bool Done { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public Guid? ListId { get; set; }
}
