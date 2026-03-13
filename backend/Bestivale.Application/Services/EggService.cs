using Bestivale.Application.Dtos;
using Bestivale.Application.Interfaces;
using Bestivale.Domain.Entities;

namespace Bestivale.Application.Services;

public sealed class EggService
{
    private readonly IEggRepository _eggRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMarketRepository _marketRepository;
    private readonly Random _rng = new();

    public EggService(IEggRepository eggRepository, IUserRepository userRepository, IMarketRepository marketRepository)
    {
        _eggRepository = eggRepository;
        _userRepository = userRepository;
        _marketRepository = marketRepository;
    }

    public async Task EnsureAtLeastEggsForAllUsersAsync(int minEggs, CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        foreach (var user in users)
        {
            var count = await _eggRepository.CountByOwnerAsync(user.Id, cancellationToken);
            if (count >= minEggs)
            {
                continue;
            }

            var missing = minEggs - count;
            var eggs = Enumerable.Range(0, missing).Select(_ => CreateRandomEgg(user.Id)).ToList();
            await _eggRepository.AddRangeAsync(eggs, cancellationToken);
        }
    }

    public async Task<IReadOnlyList<EggDto>> GetEggsForUserAsync(string username, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByUsernameAsync(username.Trim(), cancellationToken);
        if (user is null)
        {
            return Array.Empty<EggDto>();
        }

        var eggs = await _eggRepository.GetByOwnerAsync(user.Id, cancellationToken);
        return eggs
            .Where(e => !e.IsListed)
            .Select(e => new EggDto
            {
                Id = e.Id,
                TemplateCode = e.TemplateCode,
                ColorHex = e.ColorHex,
                ColorDescription = e.ColorDescription,
                CreatedAt = e.CreatedAt
            })
            .ToList();
    }

    public async Task<EggDto> GrantRandomEggForUserAsync(string username, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username is required.", nameof(username));
        }

        var user = await _userRepository.GetByUsernameAsync(username.Trim(), cancellationToken);
        if (user is null)
        {
            throw new InvalidOperationException("User does not exist.");
        }

        var egg = CreateRandomEgg(user.Id);
        await _eggRepository.AddRangeAsync(new[] { egg }, cancellationToken);

        return new EggDto
        {
            Id = egg.Id,
            TemplateCode = egg.TemplateCode,
            ColorHex = egg.ColorHex,
            ColorDescription = egg.ColorDescription,
            CreatedAt = egg.CreatedAt
        };
    }

    private Egg CreateRandomEgg(Guid ownerUserId)
    {
        var (hex, desc) = GenerateRandomColor();

        return new Egg
        {
            Id = Guid.NewGuid(),
            OwnerUserId = ownerUserId,
            TemplateCode = "EMBRYO_MUTAGEN",
            ColorHex = hex,
            ColorDescription = desc,
            CreatedAt = DateTime.UtcNow
        };
    }

    private (string Hex, string Description) GenerateRandomColor()
    {
        // Simple pastel-ish HSL -> RGB
        var hue = _rng.NextDouble() * 360.0;
        var sat = 0.45 + _rng.NextDouble() * 0.4;   // 0.45–0.85
        var light = 0.4 + _rng.NextDouble() * 0.3;  // 0.4–0.7

        var rgb = HslToRgb(hue, sat, light);
        var hex = $"#{rgb.R:X2}{rgb.G:X2}{rgb.B:X2}";

        var tone = hue switch
        {
            >= 330 or < 20 => "warm crimson",
            >= 20 and < 70 => "amber glow",
            >= 70 and < 150 => "meadow green",
            >= 150 and < 210 => "seafoam teal",
            >= 210 and < 260 => "midnight blue",
            >= 260 and < 320 => "mystic violet",
            _ => "strange shimmer"
        };

        var descriptor = light switch
        {
            < 0.45 => "deep",
            < 0.55 => "soft",
            _ => "bright"
        };

        var description = $"{descriptor} {tone}";
        return (hex, description);
    }

    private static (byte R, byte G, byte B) HslToRgb(double h, double s, double l)
    {
        h = h / 360.0;

        double r, g, b;

        if (s == 0.0)
        {
            r = g = b = l;
        }
        else
        {
            double HueToRgb(double p, double q, double t)
            {
                if (t < 0) t += 1;
                if (t > 1) t -= 1;
                if (t < 1.0 / 6.0) return p + (q - p) * 6 * t;
                if (t < 1.0 / 2.0) return q;
                if (t < 2.0 / 3.0) return p + (q - p) * (2.0 / 3.0 - t) * 6;
                return p;
            }

            var q = l < 0.5 ? l * (1 + s) : l + s - l * s;
            var p = 2 * l - q;
            r = HueToRgb(p, q, h + 1.0 / 3.0);
            g = HueToRgb(p, q, h);
            b = HueToRgb(p, q, h - 1.0 / 3.0);
        }

        byte ToByte(double v) => (byte)Math.Clamp((int)Math.Round(v * 255), 0, 255);

        return (ToByte(r), ToByte(g), ToByte(b));
    }
}

