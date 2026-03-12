using Bestivale.Application.Dtos;
using Bestivale.Application.Interfaces;
using Bestivale.Domain.Entities;

namespace Bestivale.Application.Services;

public sealed class MarketService : IMarketService
{
    private const string StatusActive = "Active";
    private const string StatusSold = "Sold";

    private readonly IMarketRepository _marketRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMonsterRepository _monsterRepository;

    public MarketService(IMarketRepository marketRepository, IUserRepository userRepository, IMonsterRepository monsterRepository)
    {
        _marketRepository = marketRepository;
        _userRepository = userRepository;
        _monsterRepository = monsterRepository;
    }

    public async Task<IReadOnlyList<MarketListingDto>> GetActiveListingsAsync(CancellationToken cancellationToken = default)
    {
        var listings = await _marketRepository.GetActiveListingsAsync(cancellationToken);
        return listings.Select(MapToDto).ToList();
    }

    public async Task<MarketListingDto> CreateListingAsync(
        string sellerUsername,
        CreateMarketListingRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.Price <= 0)
        {
            throw new ArgumentException("Price must be greater than 0.");
        }

        var seller = await _userRepository.GetByUsernameAsync(sellerUsername.Trim(), cancellationToken);
        if (seller is null)
        {
            throw new InvalidOperationException("Seller user does not exist.");
        }

        var monsters = await _monsterRepository.GetAllAsync(cancellationToken);
        var monster = monsters.FirstOrDefault(m => m.Id == request.MonsterId);
        if (monster is null)
        {
            throw new InvalidOperationException("Monster does not exist.");
        }

        var listing = new MarketListing
        {
            Id = Guid.NewGuid(),
            MonsterId = monster.Id,
            SellerUserId = seller.Id,
            Price = request.Price,
            Status = StatusActive,
            CreatedAt = DateTime.UtcNow
        };

        await _marketRepository.AddAsync(listing, cancellationToken);

        // Repo may not include navs on add; return a DTO using current data.
        return new MarketListingDto
        {
            Id = listing.Id,
            MonsterId = monster.Id,
            MonsterName = monster.Name,
            MonsterImageUrl = monster.ImageUrl,
            MonsterMythology = monster.Mythology,
            Price = listing.Price,
            Status = listing.Status,
            SellerUsername = seller.Username,
            CreatedAt = listing.CreatedAt
        };
    }

    public async Task<bool> BuyListingAsync(string buyerUsername, BuyMarketListingRequest request, CancellationToken cancellationToken = default)
    {
        var buyer = await _userRepository.GetByUsernameAsync(buyerUsername.Trim(), cancellationToken);
        if (buyer is null)
        {
            throw new InvalidOperationException("Buyer user does not exist.");
        }

        var listing = await _marketRepository.GetByIdAsync(request.ListingId, cancellationToken);
        if (listing is null || listing.Status != StatusActive)
        {
            return false;
        }

        if (listing.SellerUserId == buyer.Id)
        {
            throw new InvalidOperationException("You cannot buy your own listing.");
        }

        if (buyer.CurrencyBalance < listing.Price)
        {
            throw new InvalidOperationException("Insufficient funds.");
        }

        var seller = await _userRepository.GetByIdAsync(listing.SellerUserId, cancellationToken);
        if (seller is null)
        {
            throw new InvalidOperationException("Seller user does not exist.");
        }

        buyer.CurrencyBalance -= listing.Price;
        seller.CurrencyBalance += listing.Price;

        listing.Status = StatusSold;
        listing.SoldAt = DateTime.UtcNow;
        listing.BuyerUserId = buyer.Id;

        await _userRepository.UpdateAsync(buyer, cancellationToken);
        await _userRepository.UpdateAsync(seller, cancellationToken);
        await _marketRepository.UpdateAsync(listing, cancellationToken);

        return true;
    }

    private static MarketListingDto MapToDto(MarketListing l) =>
        new()
        {
            Id = l.Id,
            MonsterId = l.MonsterId,
            MonsterName = l.Monster?.Name ?? string.Empty,
            MonsterImageUrl = l.Monster?.ImageUrl ?? string.Empty,
            MonsterMythology = l.Monster?.Mythology ?? string.Empty,
            Price = l.Price,
            Status = l.Status,
            SellerUsername = l.SellerUser?.Username ?? string.Empty,
            CreatedAt = l.CreatedAt
        };
}

