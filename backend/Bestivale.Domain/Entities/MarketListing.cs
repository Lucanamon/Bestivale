namespace Bestivale.Domain.Entities;

public sealed class MarketListing
{
    public Guid Id { get; set; }

    // New polymorphic link: listing sells an owned InventoryItem (egg or monster).
    // During transition this may be null for legacy listings.
    public Guid? InventoryItemId { get; set; }

    public InventoryItem? InventoryItem { get; set; }

    public Guid MonsterId { get; set; }

    public Monster? Monster { get; set; }

    // When selling a specific egg instance, link to it here.
    public Guid? EggId { get; set; }

    public Egg? Egg { get; set; }

    public Guid SellerUserId { get; set; }

    public User? SellerUser { get; set; }

    public int Price { get; set; }

    public string Status { get; set; } = "Active"; // Active | Sold | Cancelled

    public DateTime CreatedAt { get; set; }

    public DateTime? SoldAt { get; set; }

    public Guid? BuyerUserId { get; set; }
}

