namespace App;

public class Models
{
    public record Product(
        string Name,
        decimal Price, 
        string Category,
        DateTimeOffset ProductionDate);
}