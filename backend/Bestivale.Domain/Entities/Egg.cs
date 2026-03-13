namespace Bestivale.Domain.Entities;

public sealed class Egg
{
    public Guid Id { get; set; }

    public Guid OwnerUserId { get; set; }
    public User? OwnerUser { get; set; }

    public string TemplateCode { get; set; } = "EMBRYO_MUTAGEN";

    public string ColorHex { get; set; } = "#ffffff";

    public string ColorDescription { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public bool IsListed { get; set; }
}

