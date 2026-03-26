namespace Bestivale.Domain.Entities;

public sealed class InventoryEgg
{
    public Guid InventoryItemId { get; set; }

    public InventoryItem? InventoryItem { get; set; }

    public string TemplateCode { get; set; } = "EMBRYO_MUTAGEN";

    public string ColorHex { get; set; } = "#ffffff";

    public string ColorDescription { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}

