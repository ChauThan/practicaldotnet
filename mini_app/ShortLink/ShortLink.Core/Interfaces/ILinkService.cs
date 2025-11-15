using ShortLink.Core.Models;

namespace ShortLink.Core.Interfaces
{
    public interface ILinkService
    {
        Task<Link> CreateShortLink(string longUrl);
        Task<Link?> GetLinkByShortCode(string shortCode);
        Task DeleteLink(string shortCode);
    }
}
