namespace TodoClient;

public class Audit : Entity
{
    public Guid UserId { get; set; }
    public AuditType Type { get; set; }
    public string? EntityName { get; set; }
    public DateTime AuditedAt { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? AffectedColumns { get; set; }
    public string? PrimaryKey { get; set; }
}
