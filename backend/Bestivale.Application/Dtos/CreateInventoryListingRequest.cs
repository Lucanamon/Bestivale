namespace Bestivale.Application.Dtos;

public sealed class CreateInventoryListingRequest
{
    public Guid InventoryItemId { get; init; }
    public int Price { get; init; }
}

