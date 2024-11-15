using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<ProductEntity> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductEntity>(b =>
        {
            b.HasKey(s => s.Id);
            b.Property(s => s.Id).ValueGeneratedOnAdd();

            b.HasData([
                new("Product 1", 100, "Cat A", new DateTime(2024, 1, 1)) {Id = 1},
                new("Product 2", 200, "Cat A", new DateTime(2024, 2, 1)) {Id = 2},
                new("Product 3", 300, "Cat A", new DateTime(2024, 3, 1)) {Id = 3},
                new("Product 4", 400, "Cat B", new DateTime(2024, 4, 1)) {Id = 4},
                new("Product 5", 500, "Cat B", new DateTime(2024, 5, 1)) {Id = 5},
                new("Product 6", 600, "Cat B", new DateTime(2024, 6, 1)) {Id = 6},
            ]);
        });
        
        base.OnModelCreating(modelBuilder);
    }
}