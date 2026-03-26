namespace Bestivale.Application.Dtos;

public sealed class InventoryItemDto
{
    public Guid Id { get; init; }
    public string ItemType { get; init; } = string.Empty; // Egg | Monster
    public bool IsFavorite { get; init; }
    public bool IsListed { get; init; }
    public DateTime CreatedAt { get; init; }

    // Egg fields (when ItemType == "Egg")
    public string? TemplateCode { get; init; }
    public string? ColorHex { get; init; }
    public string? ColorDescription { get; init; }

    // Monster fields (when ItemType == "Monster")
    public Guid? MonsterId { get; init; }
    public string? MonsterName { get; init; }
    public string? MonsterMythology { get; init; }
    public string? MonsterImageUrl { get; init; }
    public string? MonsterDescription { get; init; }
}

