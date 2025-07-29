using App.Application.Abstractions;
using App.Domain;
using Microsoft.AspNetCore.Identity;

namespace App.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ApplicationUser?> FindByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<(bool Succeeded, IEnumerable<string> Errors, Guid? UserId)> CreateUserAsync(ApplicationUser user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            return (true, Enumerable.Empty<string>(), user.Id);
        }

        return (false, result.Errors.Select(e => e.Description), null);
    }
}
