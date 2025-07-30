using App.Domain;

namespace App.Application.Abstractions;


public interface IUserService
{
    Task<ApplicationUser?> FindByEmailAsync(string email);
    Task<ApplicationUser?> FindByIdAsync(Guid id);
    Task<(bool Succeeded, IEnumerable<string> Errors, Guid? UserId)> CreateUserAsync(ApplicationUser user, string password);

    Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user);
    Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled);
    Task<string?> GetAuthenticatorKeyAsync(ApplicationUser user);
    Task ResetAuthenticatorKeyAsync(ApplicationUser user);
    Task<IEnumerable<string>> GenerateNewTwoFactorRecoveryCodesAsync(ApplicationUser user, int number);
    Task<bool> VerifyTwoFactorTokenAsync(ApplicationUser user, string code);
}
