using App.Domain;

namespace App.Application.Abstractions;

public interface IUserService
{
    Task<ApplicationUser?> FindByEmailAsync(string email);
    Task<(bool Succeeded, IEnumerable<string> Errors, Guid? UserId)> CreateUserAsync(ApplicationUser user, string password);
}
