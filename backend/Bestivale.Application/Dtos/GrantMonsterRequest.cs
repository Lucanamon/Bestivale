namespace Bestivale.Application.Dtos;

public sealed class GrantMonsterRequest
{
    public string TargetUsername { get; init; } = string.Empty;
    public Guid MonsterId { get; init; }
}

