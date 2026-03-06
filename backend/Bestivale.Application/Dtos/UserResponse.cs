namespace Bestivale.Application.Dtos;

public sealed class UserResponse
{
    public Guid Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public int CurrencyBalance { get; init; }
    public DateTime CreatedAt { get; init; }
}

