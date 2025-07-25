using App.Domain;

namespace App.Application.Abstractions;

public interface ISignInService
{
    Task<SignInResult> CheckPasswordSignInAsync(ApplicationUser user, string password, bool lockoutOnFailure = false);
}
