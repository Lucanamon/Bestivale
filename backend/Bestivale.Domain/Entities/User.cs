namespace Bestivale.Domain.Entities;

public sealed class User
{
    public Guid Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = "User";

    public int CurrencyBalance { get; set; } = 10;

    public DateTime CreatedAt { get; set; }

    public bool IsRootAdmin { get; set; }
}

