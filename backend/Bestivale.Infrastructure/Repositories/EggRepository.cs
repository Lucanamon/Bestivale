using Bestivale.Application.Interfaces;
using Bestivale.Domain.Entities;
using Bestivale.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Bestivale.Infrastructure.Repositories;

public sealed class EggRepository : IEggRepository
{
    private readonly BestivaleDbContext _db;

    public EggRepository(BestivaleDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Egg>> GetByOwnerAsync(Guid ownerUserId, CancellationToken cancellationToken = default)
    {
        return await _db.Eggs
            .AsNoTracking()
            .Where(e => e.OwnerUserId == ownerUserId)
            .OrderBy(e => e.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Egg>> GetFavoritesByOwnerAsync(Guid ownerUserId, CancellationToken cancellationToken = default)
    {
        return await _db.Eggs
            .AsNoTracking()
            .Where(e => e.OwnerUserId == ownerUserId && e.IsFavorite)
            .OrderBy(e => e.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountByOwnerAsync(Guid ownerUserId, CancellationToken cancellationToken = default)
    {
        return await _db.Eggs.CountAsync(e => e.OwnerUserId == ownerUserId, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<Egg> eggs, CancellationToken cancellationToken = default)
    {
        _db.Eggs.AddRange(eggs);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<Egg?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _db.Eggs.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(Egg egg, CancellationToken cancellationToken = default)
    {
        _db.Eggs.Update(egg);
        await _db.SaveChangesAsync(cancellationToken);
    }
}

