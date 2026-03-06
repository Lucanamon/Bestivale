using Bestivale.Domain.Entities;

namespace Bestivale.Application.Interfaces;

public interface IMonsterRepository
{
    Task<IReadOnlyList<Monster>> GetAllAsync(CancellationToken cancellationToken = default);
}

