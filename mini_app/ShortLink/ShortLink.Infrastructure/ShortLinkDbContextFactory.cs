using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ShortLink.Infrastructure;

public class ShortLinkDbContextFactory : IDesignTimeDbContextFactory<ShortLinkDbContext>
{
    public ShortLinkDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<ShortLinkDbContext>();
        // default to a file database for design-time tooling
        builder.UseSqlite("Data Source=shortlink.db");
        return new ShortLinkDbContext(builder.Options);
    }
}
