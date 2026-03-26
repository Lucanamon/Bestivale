namespace Bestivale.Domain.Entities;

public sealed class InventoryItem
{
    public Guid Id { get; set; }

    public Guid OwnerUserId { get; set; }

    public User? OwnerUser { get; set; }

    public InventoryItemType ItemType { get; set; }

    public bool IsFavorite { get; set; }

    public bool IsListed { get; set; }

    public DateTime CreatedAt { get; set; }

    public InventoryEgg? InventoryEgg { get; set; }

    public InventoryMonster? InventoryMonster { get; set; }
}

