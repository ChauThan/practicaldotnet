using Microsoft.EntityFrameworkCore;

namespace TodoClient;

public class AppDbContext : DbContext
{
    public DbSet<TodoItem> TodoItems { get; set; }
    public DbSet<TodoList> TodoLists { get; set; }

    public AppDbContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("TodoClient");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoItem>(
            b =>
            {
                b.HasKey(s => s.Id);
                b.Property(s => s.Title).IsRequired();

                b.HasOne<TodoList>()
                    .WithMany()
                    .HasForeignKey(s => s.ListId);
            }
        );

        modelBuilder.Entity<TodoList>(
            b =>
            {
                b.HasKey(s => s.Id);
                b.Property(s => s.Title).IsRequired();
            }
        );

        modelBuilder.Seed();
        
        base.OnModelCreating(modelBuilder);
    }
}
