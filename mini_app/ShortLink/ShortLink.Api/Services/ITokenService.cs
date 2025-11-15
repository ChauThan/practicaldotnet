using ShortLink.Infrastructure.Identity;

namespace ShortLink.Api.Services;

public interface ITokenService
{
    Task<string> CreateTokenAsync(ApplicationUser user);
}
