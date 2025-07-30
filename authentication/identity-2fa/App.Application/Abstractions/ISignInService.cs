using App.Domain;

namespace App.Application.Abstractions;


public interface ISignInService
{
    Task<SignInResult> PasswordSignInAsync(string userName, string password, bool rememberMe, bool lockoutOnFailure = false);

    /// <summary>
    /// Checks the 2FA code for a user (TOTP authenticator app).
    /// </summary>
    Task<SignInResult> TwoFactorAuthenticatorSignInAsync(ApplicationUser user, string code, bool rememberMe, bool rememberClient);
}
