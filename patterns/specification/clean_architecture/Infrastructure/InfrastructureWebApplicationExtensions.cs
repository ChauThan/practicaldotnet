using App.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.AspNetCore.Builder;

public static class InfrastructureWebApplicationExtensions
{
    public static WebApplication UseInfrastructure(this WebApplication app)
    {
        ApplyMigrations(app.Services);

        return app;
    }

    private static void ApplyMigrations(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
    }
}