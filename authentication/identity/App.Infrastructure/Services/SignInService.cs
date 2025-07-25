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

    public async Task<App.Domain.SignInResult> CheckPasswordSignInAsync(ApplicationUser user, string password, bool lockoutOnFailure = false)
    {
        var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure);

        return new App.Domain.SignInResult
        {
            Succeeded = result.Succeeded,
            IsLockedOut = result.IsLockedOut,
            IsNotAllowed = result.IsNotAllowed
        };
    }
}
