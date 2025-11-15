using ShortLink.Core.Models;

namespace ShortLink.Core.Interfaces
{
    public interface ILinkRepository
    {
        Task<Link> AddLink(Link link);
        Task<Link?> GetLinkById(Guid id);
        Task<Link?> GetByShortCode(string shortCode);
        Task UpdateLink(Link link);
        Task<IEnumerable<Link>> GetAllLinks();
        Task DeleteLink(Guid id);
    }
}