using App.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
{
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>().ToTable("Users", SchemaName.Identity);
        builder.Entity<ApplicationRole>().ToTable("Roles", SchemaName.Identity);
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles", SchemaName.Identity);
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims", SchemaName.Identity);
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins", SchemaName.Identity);
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims", SchemaName.Identity);
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens", SchemaName.Identity);

        builder.Entity<Product>().ToTable("Products", SchemaName.Sales);
        builder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);

        builder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("RefreshTokens", SchemaName.Identity);

            entity.HasKey(rt => rt.Id);

            entity.Property(rt => rt.Token)
                  .IsRequired()
                  .HasMaxLength(256);
            entity.Property(rt => rt.JwtId)
                  .IsRequired()
                  .HasMaxLength(256);

            entity.Property(rt => rt.CreationDate)
                  .IsRequired();

            entity.Property(rt => rt.ExpiryDate)
                  .IsRequired();

            entity.HasOne(rt => rt.User)
                  .WithMany(u => u.RefreshTokens)
                  .HasForeignKey(rt => rt.UserId)
                  .IsRequired()
                  .OnDelete(DeleteBehavior.Cascade);
        });

    }

    private static class SchemaName
    {
        public const string Identity = nameof(Identity);
        public const string Sales = nameof(Sales);
    }
}