using App.Infrastructure.Identity.Models;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace App.Infrastructure.Identity.IdentityServerConfig.Services;
public class ProfileService(UserManager<ApplicationUser> userManager) : IProfileService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var user = await _userManager.GetUserAsync(context.Subject);
        if (user == null)
        {
            return;
        }

        var claims = new List<Claim>
        {
            new("sub", user.Id)
        };

        var userClaims = await _userManager.GetClaimsAsync(user);
        claims.AddRange(userClaims);

        context.IssuedClaims = claims.Where(c => context.RequestedClaimTypes.Contains(c.Type)).ToList();
    }

    public Task IsActiveAsync(IsActiveContext context)
    {
        context.IsActive = true;
        return Task.CompletedTask;
    }
}