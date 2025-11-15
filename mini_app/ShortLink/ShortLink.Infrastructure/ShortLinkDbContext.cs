using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShortLink.Infrastructure.Identity;
using ShortLink.Core.Models;

namespace ShortLink.Infrastructure;

public class ShortLinkDbContext : IdentityDbContext<ApplicationUser>
{
    public ShortLinkDbContext(DbContextOptions<ShortLinkDbContext> options)
        : base(options) { }

    public DbSet<Link> Links { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Link>(b =>
        {
            b.HasKey(l => l.Id);
            b.HasIndex(l => l.ShortCode).IsUnique();
            b.Property(l => l.ShortCode).IsRequired().HasMaxLength(128);
            b.Property(l => l.OriginalUrl).IsRequired();
        });
    }
}
