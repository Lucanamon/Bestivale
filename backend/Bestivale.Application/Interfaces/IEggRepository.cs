using Bestivale.Domain.Entities;

namespace Bestivale.Application.Interfaces;

public interface IEggRepository
{
    Task<IReadOnlyList<Egg>> GetByOwnerAsync(Guid ownerUserId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Egg>> GetFavoritesByOwnerAsync(Guid ownerUserId, CancellationToken cancellationToken = default);
    Task<int> CountByOwnerAsync(Guid ownerUserId, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<Egg> eggs, CancellationToken cancellationToken = default);
    Task<Egg?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateAsync(Egg egg, CancellationToken cancellationToken = default);
}

