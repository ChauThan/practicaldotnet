using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using App.Infrastructure.Persistence;
using App.Domain;
using Microsoft.AspNetCore.Identity;

namespace App.IntegrationTests.Helpers;

public class ApiWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    private const string TestDbName = "App_IntegrationTestDb";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove all ApplicationDbContext, DbContextOptions, and SqlServer provider registrations
            var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            var connectionString = $"Server=(localdb)\\mssqllocaldb;Database={TestDbName};Trusted_Connection=True;MultipleActiveResultSets=true";
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });


            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();

            var scopedServices = scope.ServiceProvider;
            var dbContext = scopedServices.GetRequiredService<ApplicationDbContext>();
            var userManager = scopedServices.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scopedServices.GetRequiredService<RoleManager<ApplicationRole>>();

            // Ensure the database is created fresh for each test run
            dbContext.Database.EnsureDeleted();
            dbContext.Database.Migrate();

            SeedData(userManager, roleManager).Wait();
        });
    }
    private async Task SeedData(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        if (!await roleManager.RoleExistsAsync("User"))
        {
            await roleManager.CreateAsync(new ApplicationRole { Name = "User" });
        }

        var testUser = await userManager.FindByEmailAsync("integration.test@example.com");
        if (testUser == null)
        {
            testUser = new ApplicationUser
            {
                UserName = "integration.test@example.com",
                Email = "integration.test@example.com",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(testUser, "Test@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(testUser, "User");
            }
            else
            {
                throw new Exception($"Failed to create test user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }
}
