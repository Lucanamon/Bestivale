namespace Bestivale.Application.Dtos;

public sealed class EggDto
{
    public Guid Id { get; init; }
    public string TemplateCode { get; init; } = string.Empty;
    public string ColorHex { get; init; } = string.Empty;
    public string ColorDescription { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public bool IsFavorite { get; init; }
}

