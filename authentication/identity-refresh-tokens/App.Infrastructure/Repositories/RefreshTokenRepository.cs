using App.Application.Repositories;
using App.Domain;
using App.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext dbContext;
    public RefreshTokenRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<RefreshToken>()
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
    }

    public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        await dbContext.Set<RefreshToken>().AddAsync(refreshToken, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        dbContext.Set<RefreshToken>().Update(refreshToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<RefreshToken>()
            .Where(rt => rt.UserId == userId && rt.ExpiryDate > DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }

    public async Task RevokeAllTokensForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var tokens = await dbContext.Set<RefreshToken>()
            .Where(rt => rt.UserId == userId && rt.RevokedDate == null && rt.ExpiryDate > DateTime.UtcNow)
            .ToListAsync(cancellationToken);
        foreach (var token in tokens)
        {
            token.RevokedDate = DateTime.UtcNow;
        }
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var token = await dbContext.Set<RefreshToken>()
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.RevokedDate == null && rt.ExpiryDate > DateTime.UtcNow, cancellationToken);
        if (token != null)
        {
            token.RevokedDate = DateTime.UtcNow;
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
