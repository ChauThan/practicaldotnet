namespace TodoClient;

public class TodoItem : Entity
{
    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public bool Done { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset DueTo { get; set; }

    public Guid? ListId { get; set; }
}
