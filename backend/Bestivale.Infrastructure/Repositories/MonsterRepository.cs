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
    public async Task<Monster?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
    return await _db.Monsters.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(Monster monster, CancellationToken cancellationToken = default)
    {
        _db.Monsters.Update(monster);
        await _db.SaveChangesAsync(cancellationToken);
    }
}

