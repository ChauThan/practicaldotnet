using ShortLink.Core.Interfaces;
using ShortLink.Core.Models;

namespace ShortLink.Core.Services
{
    // In-memory repository to store links; this used to be LinkService but is now a repository implementation
    public class InMemoryLinkRepository : ILinkRepository
    {
        private readonly List<Link> _links = new List<Link>();
        private readonly object _lock = new object();

        public Task<Link> AddLink(Link link)
        {
            link.Id = Guid.NewGuid();
            lock (_lock)
            {
                _links.Add(link);
            }
            return Task.FromResult(link);
        }

        public Task<Link?> GetLinkById(Guid id)
        {
            var link = _links.FirstOrDefault(l => l.Id == id);
            return Task.FromResult(link);
        }

        public Task<Link?> GetByShortCode(string shortCode)
        {
            var link = _links.FirstOrDefault(l => string.Equals(l.ShortCode, shortCode, StringComparison.Ordinal));
            return Task.FromResult(link);
        }

        public Task<IEnumerable<Link>> GetAllLinks()
        {
            return Task.FromResult<IEnumerable<Link>>(_links);
        }

        public Task DeleteLink(Guid id)
        {
            var link = _links.FirstOrDefault(l => l.Id == id);
            if (link != null)
            {
                lock (_lock)
                {
                    _links.Remove(link);
                }
            }
            return Task.CompletedTask;
        }

        public Task UpdateLink(Link link)
        {
            lock (_lock)
            {
                var existing = _links.FirstOrDefault(l => l.Id == link.Id);
                if (existing != null)
                {
                    existing.OriginalUrl = link.OriginalUrl;
                    existing.ShortCode = link.ShortCode;
                    existing.DateCreated = link.DateCreated;
                    existing.HitCount = link.HitCount;
                    existing.LastAccessed = link.LastAccessed;
                }
            }
            return Task.CompletedTask;
        }
    }
}
