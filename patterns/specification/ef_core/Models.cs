namespace App;

public class Models
{
    public class Product(
        string name,
        decimal price,
        string category,
        DateTime productionDate)
    {
        public int Id { get; set; }
        public string Name { get; set; } = name;
        public decimal Price { get; set; } = price;
        public string Category { get; set; } = category;
        public DateTime ProductionDate { get; set; } = productionDate;
        
    };
}