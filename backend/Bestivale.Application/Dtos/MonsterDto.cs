namespace Bestivale.Application.Dtos;

public sealed class MonsterDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Mythology { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = string.Empty;
}

