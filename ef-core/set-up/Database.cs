using EFCore.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCore;

public class Database(DbContextOptions options) : DbContext(options)
{
    public DbSet<Post> Posts { get; set; }
    public DbSet<Author> Authors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>(b =>
        {
            b.HasKey(s => s.Id);
            b.Property(s => s.Id).ValueGeneratedOnAdd();
            
            b.HasOne(s => s.Author)
                .WithMany()
                .HasForeignKey(s => s.AuthorId);
        });

        modelBuilder.Entity<Author>(b =>
        {
            b.HasKey(s => s.Id);
            b.Property(s => s.Id).ValueGeneratedOnAdd();
        });
        
        base.OnModelCreating(modelBuilder);
    }
}