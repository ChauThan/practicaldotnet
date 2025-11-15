using ShortLink.Core.Interfaces;
using ShortLink.Core.Models;

namespace ShortLink.Core.Services
{
    // In-memory repository to store links; this used to be LinkService but is now a repository implementation
    public class InMemoryLinkRepository : ILinkRepository
    {
        private readonly List<Link> _links = new List<Link>();

        public Task<Link> AddLink(Link link)
        {
            link.Id = Guid.NewGuid();
            _links.Add(link);
            return Task.FromResult(link);
        }

        public Task<Link?> GetLinkById(Guid id)
        {
            var link = _links.FirstOrDefault(l => l.Id == id);
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
                _links.Remove(link);
            }
            return Task.CompletedTask;
        }
    }
}
