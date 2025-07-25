﻿namespace App.Domain;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }

    public Guid? CreatedById { get; set; }
    public ApplicationUser? CreatedBy { get; set; }
}
