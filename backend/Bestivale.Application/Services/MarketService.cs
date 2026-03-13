using Bestivale.Application.Dtos;
using Bestivale.Application.Interfaces;
using Bestivale.Domain.Entities;

namespace Bestivale.Application.Services;

public sealed class MarketService : IMarketService
{
    private const string StatusActive = "Active";
    private const string StatusSold = "Sold";
    private const string StatusCancelled = "Cancelled";

    private readonly IMarketRepository _marketRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMonsterRepository _monsterRepository;
    private readonly IEggRepository _eggRepository;

    public MarketService(IMarketRepository marketRepository, IUserRepository userRepository, IMonsterRepository monsterRepository, IEggRepository eggRepository)
    {
        _marketRepository = marketRepository;
        _userRepository = userRepository;
        _monsterRepository = monsterRepository;
        _eggRepository = eggRepository;
    }

    public async Task<IReadOnlyList<MarketListingDto>> GetActiveListingsAsync(CancellationToken cancellationToken = default)
    {
        var listings = await _marketRepository.GetActiveListingsAsync(cancellationToken);
        return listings.Select(MapToDto).ToList();
    }

    public async Task<MarketListingDto> CreateInventoryListingAsync(
        string sellerUsername,
        CreateInventoryListingRequest request,
        CancellationToken cancellationToken = default)
    {
        var eggRequest = new CreateEggListingRequest
        {
            EggId = request.InventoryItemId,
            Price = request.Price
        };

        return await CreateEggListingAsync(sellerUsername, eggRequest, cancellationToken);
    }

    public async Task<MarketListingDto> CreateEggListingAsync(
        string sellerUsername,
        CreateEggListingRequest request,
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

        var egg = await _eggRepository.GetByIdAsync(request.EggId, cancellationToken);
        if (egg is null || egg.OwnerUserId != seller.Id)
        {
            throw new InvalidOperationException("Egg does not exist or is not owned by seller.");
        }

        // Ensure egg is not already listed
        if (egg.IsListed)
        {
            throw new InvalidOperationException("This egg is already listed.");
        }

        // Use the registered "Embryo mutagen egg" monster as the visual template
        var monsters = await _monsterRepository.GetAllAsync(cancellationToken);
        var monster = monsters.FirstOrDefault(m => m.Name == "Embryo mutagen egg");
        if (monster is null)
        {
            throw new InvalidOperationException("Base egg monster is not registered.");
        }

        var listing = new MarketListing
        {
            Id = Guid.NewGuid(),
            MonsterId = monster.Id,
            EggId = egg.Id,
            SellerUserId = seller.Id,
            Price = request.Price,
            Status = StatusActive,
            CreatedAt = DateTime.UtcNow
        };

        await _marketRepository.AddAsync(listing, cancellationToken);
        egg.IsListed = true;
        await _eggRepository.UpdateAsync(egg, cancellationToken);

        return new MarketListingDto
        {
            Id = listing.Id,
            MonsterId = monster.Id,
            MonsterName = monster.Name,
            MonsterImageUrl = monster.ImageUrl,
            MonsterMythology = monster.Mythology,
            Price = listing.Price,
            Status = listing.Status,
            SellerUserId = seller.Id,
            SellerUsername = seller.Username,
            CreatedAt = listing.CreatedAt
        };
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

        // If this listing represents a specific egg, transfer ownership
        if (listing.EggId is Guid eggId)
        {
            var egg = await _eggRepository.GetByIdAsync(eggId, cancellationToken);
            if (egg is not null)
            {
                egg.OwnerUserId = buyer.Id;
                egg.IsListed = false;
                await _eggRepository.UpdateAsync(egg, cancellationToken);
            }
        }

        await _userRepository.UpdateAsync(buyer, cancellationToken);
        await _userRepository.UpdateAsync(seller, cancellationToken);
        await _marketRepository.UpdateAsync(listing, cancellationToken);

        return true;
    }

    public async Task<bool> CancelListingAsync(string sellerUsername, Guid listingId, CancellationToken cancellationToken = default)
    {
        var seller = await _userRepository.GetByUsernameAsync(sellerUsername.Trim(), cancellationToken);
        if (seller is null)
        {
            throw new InvalidOperationException("Seller user does not exist.");
        }

        var listing = await _marketRepository.GetByIdAsync(listingId, cancellationToken);
        if (listing is null || listing.Status != StatusActive)
        {
            return false;
        }

        if (listing.SellerUserId != seller.Id)
        {
            throw new InvalidOperationException("You can only cancel your own listings.");
        }

        listing.Status = StatusCancelled;
        await _marketRepository.UpdateAsync(listing, cancellationToken);

        // Return egg to seller inventory
        if (listing.EggId is Guid eggId)
        {
            var egg = await _eggRepository.GetByIdAsync(eggId, cancellationToken);
            if (egg is not null)
            {
                egg.IsListed = false;
                await _eggRepository.UpdateAsync(egg, cancellationToken);
            }
        }

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
            SellerUserId = l.SellerUserId,
            SellerUsername = l.SellerUser?.Username ?? string.Empty,
            CreatedAt = l.CreatedAt
        };
}

