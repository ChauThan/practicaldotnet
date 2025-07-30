using App.Application.Abstractions;
using App.Domain;
using Microsoft.AspNetCore.Identity;

namespace App.Infrastructure.Services;

public class SignInService : ISignInService
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public SignInService(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<App.Domain.SignInResult> PasswordSignInAsync(string userName, string password, bool rememberMe, bool lockoutOnFailure = false)
    {
        var result = await _signInManager.PasswordSignInAsync(userName, password, rememberMe, lockoutOnFailure);

        bool requiresTwoFactor = result.RequiresTwoFactor;
        return new App.Domain.SignInResult
        {
            Succeeded = !requiresTwoFactor && result.Succeeded,
            IsLockedOut = result.IsLockedOut,
            IsNotAllowed = result.IsNotAllowed,
            RequiresTwoFactor = requiresTwoFactor
        };
    }

    public async Task<App.Domain.SignInResult> TwoFactorAuthenticatorSignInAsync(ApplicationUser user, string code, bool rememberMe, bool rememberClient)
    {
        var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(code, rememberMe, rememberClient);
        return new App.Domain.SignInResult
        {
            Succeeded = result.Succeeded,
            IsLockedOut = result.IsLockedOut,
            IsNotAllowed = result.IsNotAllowed,
            RequiresTwoFactor = result.RequiresTwoFactor
        };
    }
}
