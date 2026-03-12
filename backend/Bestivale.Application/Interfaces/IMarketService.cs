using Bestivale.Application.Dtos;

namespace Bestivale.Application.Interfaces;

public interface IMarketService
{
    Task<IReadOnlyList<MarketListingDto>> GetActiveListingsAsync(CancellationToken cancellationToken = default);
    Task<MarketListingDto> CreateListingAsync(string sellerUsername, CreateMarketListingRequest request, CancellationToken cancellationToken = default);
    Task<bool> BuyListingAsync(string buyerUsername, BuyMarketListingRequest request, CancellationToken cancellationToken = default);
}

