using App.Infrastructure.Identity.Data;
using App.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace App.WebAPI;

public static class WebApplicationExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var demoUser = await userManager.FindByNameAsync("demo_user");
        if (demoUser == null)
        {
            var user = new ApplicationUser
            {
                UserName = "demo_user",
                Email = "demo@example.com",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(user, "Pass123$");
            if (result.Succeeded)
            {
                await userManager.AddClaimAsync(user, new Claim("name", "Demo User"));
                await userManager.AddClaimAsync(user, new Claim("preferred_username", "demouser_pro"));
                await userManager.AddClaimAsync(user, new Claim("email", user.Email!));
            }
        }
    }
}
