using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using static App.Models;

namespace App;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(b =>
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

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite("Data Source=app.db");

        return new AppDbContext(optionsBuilder.Options);
    }
}