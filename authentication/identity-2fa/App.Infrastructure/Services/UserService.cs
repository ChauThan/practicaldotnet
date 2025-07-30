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
        => await _userManager.FindByEmailAsync(email);

    public async Task<ApplicationUser?> FindByIdAsync(Guid id)
        => await _userManager.FindByIdAsync(id.ToString());

    public async Task<(bool Succeeded, IEnumerable<string> Errors, Guid? UserId)> CreateUserAsync(ApplicationUser user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            return (true, Enumerable.Empty<string>(), user.Id);
        }

        return (false, result.Errors.Select(e => e.Description), null);
    }

    public async Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user)
        => await _userManager.GetTwoFactorEnabledAsync(user);

    public async Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled)
        => await _userManager.SetTwoFactorEnabledAsync(user, enabled);

    public async Task<string?> GetAuthenticatorKeyAsync(ApplicationUser user)
        => await _userManager.GetAuthenticatorKeyAsync(user);

    public async Task ResetAuthenticatorKeyAsync(ApplicationUser user)
        => await _userManager.ResetAuthenticatorKeyAsync(user);

    public async Task<IEnumerable<string>> GenerateNewTwoFactorRecoveryCodesAsync(ApplicationUser user, int number)
        => (await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, number)) ?? [];

    public async Task<bool> VerifyTwoFactorTokenAsync(ApplicationUser user, string code)
        => await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, code);
}
