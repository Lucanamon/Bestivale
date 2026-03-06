using Bestivale.Application.Dtos;

namespace Bestivale.Application.Interfaces;

public interface IMonsterService
{
    Task<IReadOnlyList<MonsterDto>> GetAllAsync(CancellationToken cancellationToken = default);
}

