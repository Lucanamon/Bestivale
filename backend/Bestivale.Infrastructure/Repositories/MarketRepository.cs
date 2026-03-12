using Bestivale.Application.Interfaces;
using Bestivale.Domain.Entities;
using Bestivale.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Bestivale.Infrastructure.Repositories;

public sealed class MarketRepository : IMarketRepository
{
    private readonly BestivaleDbContext _db;

    public MarketRepository(BestivaleDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<MarketListing>> GetActiveListingsAsync(CancellationToken cancellationToken = default)
    {
        return await _db.MarketListings
            .AsNoTracking()
            .Include(l => l.Monster)
            .Include(l => l.SellerUser)
            .Where(l => l.Status == "Active")
            .OrderBy(l => l.Price)
            .ThenByDescending(l => l.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<MarketListing?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _db.MarketListings.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task AddAsync(MarketListing listing, CancellationToken cancellationToken = default)
    {
        _db.MarketListings.Add(listing);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(MarketListing listing, CancellationToken cancellationToken = default)
    {
        _db.MarketListings.Update(listing);
        await _db.SaveChangesAsync(cancellationToken);
    }
}

