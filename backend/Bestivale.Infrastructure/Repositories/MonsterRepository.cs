using Bestivale.Application.Interfaces;
using Bestivale.Domain.Entities;
using Bestivale.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Bestivale.Infrastructure.Repositories;

public sealed class MonsterRepository : IMonsterRepository
{
    private readonly BestivaleDbContext _db;

    public MonsterRepository(BestivaleDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Monster>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Monsters
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}

