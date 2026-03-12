namespace Bestivale.Application.Dtos;

public sealed class CreateMarketListingRequest
{
    public Guid MonsterId { get; init; }
    public int Price { get; init; }
}

