using Bestivale.Application.Dtos;
using Bestivale.Application.Interfaces;
using Bestivale.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Bestivale.Infrastructure.Services;

public sealed class MonsterService : IMonsterService
{
    private readonly BestivaleDbContext _db;

    public MonsterService(BestivaleDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<MonsterDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Monsters
            .AsNoTracking()
            .OrderBy(m => m.Name)
            .Select(m => new MonsterDto
            {
                Id = m.Id,
                Name = m.Name,
                Mythology = m.Mythology,
                Description = m.Description,
                ImageUrl = m.ImageUrl
            })
            .ToListAsync(cancellationToken);
    }
}

