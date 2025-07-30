using App.Application.Abstractions;
using App.Domain;
using Microsoft.AspNetCore.Identity;

namespace App.Infrastructure.Services;

public class RoleService : IRoleService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public RoleService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<bool> RoleExistsAsync(string roleName)
    {
        return await _roleManager.RoleExistsAsync(roleName);
    }

    public async Task<(bool Succeeded, IEnumerable<string> Errors, Guid? RoleId)> CreateRoleAsync(string roleName)
    {
        var role = new ApplicationRole(roleName);
        var result = await _roleManager.CreateAsync(role);

        if (result.Succeeded)
        {
            return (true, Enumerable.Empty<string>(), role.Id);
        }

        return (false, result.Errors.Select(e => e.Description), null);
    }

    public async Task<ApplicationUser?> FindUserByIdAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }

    public async Task<(bool Succeeded, IEnumerable<string> Errors)> AddUserToRoleAsync(ApplicationUser user, string roleName)
    {
        var result = await _userManager.AddToRoleAsync(user, roleName);

        if (result.Succeeded)
        {
            return (true, Enumerable.Empty<string>());
        }

        return (false, result.Errors.Select(e => e.Description));
    }
}
