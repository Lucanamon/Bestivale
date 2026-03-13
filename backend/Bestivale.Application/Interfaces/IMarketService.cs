using Bestivale.Application.Dtos;

namespace Bestivale.Application.Interfaces;

public interface IMarketService
{
    Task<IReadOnlyList<MarketListingDto>> GetActiveListingsAsync(CancellationToken cancellationToken = default);
    Task<MarketListingDto> CreateListingAsync(string sellerUsername, CreateMarketListingRequest request, CancellationToken cancellationToken = default);
    Task<MarketListingDto> CreateEggListingAsync(string sellerUsername, CreateEggListingRequest request, CancellationToken cancellationToken = default);
    Task<MarketListingDto> CreateInventoryListingAsync(string sellerUsername, CreateInventoryListingRequest request, CancellationToken cancellationToken = default);
    Task<bool> BuyListingAsync(string buyerUsername, BuyMarketListingRequest request, CancellationToken cancellationToken = default);
    Task<bool> CancelListingAsync(string sellerUsername, Guid listingId, CancellationToken cancellationToken = default);
}

