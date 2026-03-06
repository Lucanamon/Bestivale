using Bestivale.Application.Dtos;
using Bestivale.Application.Interfaces;

namespace Bestivale.Application.Services;

public sealed class MonsterService : IMonsterService
{
    private readonly IMonsterRepository _monsterRepository;

    public MonsterService(IMonsterRepository monsterRepository)
    {
        _monsterRepository = monsterRepository;
    }

    public async Task<IReadOnlyList<MonsterDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var monsters = await _monsterRepository.GetAllAsync(cancellationToken);

        return monsters
            .OrderBy(m => m.Name)
            .Select(m => new MonsterDto
            {
                Id = m.Id,
                Name = m.Name,
                Mythology = m.Mythology,
                Description = m.Description,
                ImageUrl = m.ImageUrl
            })
            .ToList();
    }
}

