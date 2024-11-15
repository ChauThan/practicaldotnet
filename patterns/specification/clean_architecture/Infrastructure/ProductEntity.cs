namespace App.Infrastructure;

public class ProductEntity(
    string productName,
    decimal price,
    string category,
    DateTime productionDate)
{
    public ProductEntity() : this(string.Empty, 0, string.Empty, DateTime.MinValue)
    { }
    
    public int Id { get; set; }
    public string ProductName { get; set; } = productName;
    public decimal Price { get; set; } = price;
    public string Category { get; set; } = category;
    public DateTime ProductionDate { get; set; } = productionDate;
}