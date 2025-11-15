using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using ShortLink.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ShortLink.Infrastructure;
using ShortLink.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        // Run Identity seeding for Admin user and role if EF is enabled
        using (var scope = host.Services.CreateScope())
        {
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var useEf = config.GetValue<bool>("UseEfRepository", false);
            if (useEf)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                try
                {
                    // Migrate DB & seed identity objects
                    var db = scope.ServiceProvider.GetRequiredService<ShortLinkDbContext>();
                    db.Database.Migrate();
                    IdentitySeeder.SeedAsync(scope.ServiceProvider, config).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error during seed");
                }
            }
        }

        host.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
