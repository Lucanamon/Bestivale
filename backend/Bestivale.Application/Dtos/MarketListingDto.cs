namespace Bestivale.Application.Dtos;

public sealed class MarketListingDto
{
    public Guid Id { get; init; }
    public Guid MonsterId { get; init; }
    public string MonsterName { get; init; } = string.Empty;
    public string MonsterImageUrl { get; init; } = string.Empty;
    public string MonsterMythology { get; init; } = string.Empty;
    public int Price { get; init; }
    public string Status { get; init; } = string.Empty;
    public string SellerUsername { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}

