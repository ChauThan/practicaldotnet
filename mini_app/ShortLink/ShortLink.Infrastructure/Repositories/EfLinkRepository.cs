using Microsoft.EntityFrameworkCore;
using ShortLink.Core.Interfaces;
using ShortLink.Core.Models;

namespace ShortLink.Infrastructure.Repositories;

public class EfLinkRepository : ILinkRepository
{
    private readonly ShortLinkDbContext _db;

    public EfLinkRepository(ShortLinkDbContext db)
    {
        _db = db;
    }

    public async Task<Link> AddLink(Link link)
    {
        if (link.Id == Guid.Empty) link.Id = Guid.NewGuid();
        await _db.Links.AddAsync(link);
        await _db.SaveChangesAsync();
        return link;
    }

    public async Task DeleteLink(Guid id)
    {
        var link = await _db.Links.FindAsync(id);
        if (link != null)
        {
            _db.Links.Remove(link);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Link>> GetAllLinks()
    {
        return await _db.Links.AsNoTracking().ToListAsync();
    }

    public async Task<Link?> GetByShortCode(string shortCode)
    {
        return await _db.Links.AsNoTracking().FirstOrDefaultAsync(l => l.ShortCode == shortCode);
    }

    public async Task<Link?> GetLinkById(Guid id)
    {
        return await _db.Links.FindAsync(id);
    }

    public async Task UpdateLink(Link link)
    {
        var existing = await _db.Links.FindAsync(link.Id);
        if (existing == null) return;

        existing.OriginalUrl = link.OriginalUrl;
        existing.ShortCode = link.ShortCode;
        existing.DateCreated = link.DateCreated;
        existing.HitCount = link.HitCount;
        existing.LastAccessed = link.LastAccessed;

        await _db.SaveChangesAsync();
    }
}
