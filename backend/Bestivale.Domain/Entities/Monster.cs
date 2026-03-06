namespace Bestivale.Domain.Entities;

public sealed class Monster
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Mythology { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;
}

