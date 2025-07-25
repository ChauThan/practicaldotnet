using App.Domain;

namespace App.Application.Abstractions;

public interface IRoleService
{
    Task<bool> RoleExistsAsync(string roleName);
    Task<(bool Succeeded, IEnumerable<string> Errors, Guid? RoleId)> CreateRoleAsync(string roleName);
    Task<ApplicationUser?> FindUserByIdAsync(string userId);
    Task<(bool Succeeded, IEnumerable<string> Errors)> AddUserToRoleAsync(ApplicationUser user, string roleName);
}
