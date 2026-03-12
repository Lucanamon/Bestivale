using Bestivale.Domain.Entities;

namespace Bestivale.Application.Interfaces;

public interface IMarketRepository
{
    Task<IReadOnlyList<MarketListing>> GetActiveListingsAsync(CancellationToken cancellationToken = default);
    Task<MarketListing?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(MarketListing listing, CancellationToken cancellationToken = default);
    Task UpdateAsync(MarketListing listing, CancellationToken cancellationToken = default);
}

