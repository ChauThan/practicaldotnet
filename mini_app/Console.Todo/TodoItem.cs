namespace Console.Todo;

internal class TodoItem
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string ListName { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; init; }
}