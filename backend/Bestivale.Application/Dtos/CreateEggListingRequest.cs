namespace Bestivale.Application.Dtos;

public sealed class CreateEggListingRequest
{
    public Guid EggId { get; init; }
    public int Price { get; init; }
}

