using App.Domain;
using System.Security.Claims;

namespace App.Application.Abstractions;

public interface IJwtService
{
    string GenerateToken(ApplicationUser user, IEnumerable<Claim>? additionalClaims = null);
}
