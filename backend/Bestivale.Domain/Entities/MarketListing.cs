namespace Bestivale.Domain.Entities;

public sealed class MarketListing
{
    public Guid Id { get; set; }

    public Guid MonsterId { get; set; }

    public Monster? Monster { get; set; }

    public Guid SellerUserId { get; set; }

    public User? SellerUser { get; set; }

    public int Price { get; set; }

    public string Status { get; set; } = "Active"; // Active | Sold | Cancelled

    public DateTime CreatedAt { get; set; }

    public DateTime? SoldAt { get; set; }

    public Guid? BuyerUserId { get; set; }
}

