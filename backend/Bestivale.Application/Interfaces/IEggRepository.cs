using Bestivale.Domain.Entities;

namespace Bestivale.Application.Interfaces;

public interface IEggRepository
{
    Task<IReadOnlyList<Egg>> GetByOwnerAsync(Guid ownerUserId, CancellationToken cancellationToken = default);
    Task<int> CountByOwnerAsync(Guid ownerUserId, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<Egg> eggs, CancellationToken cancellationToken = default);
}

