namespace Bestivale.Domain.Entities;

public sealed class InventoryMonster
{
    public Guid InventoryItemId { get; set; }

    public InventoryItem? InventoryItem { get; set; }

    public Guid MonsterId { get; set; }

    public Monster? Monster { get; set; }
}

