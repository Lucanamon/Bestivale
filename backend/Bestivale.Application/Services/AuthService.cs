using Bestivale.Application.Dtos;
using Bestivale.Application.Interfaces;
using Bestivale.Domain.Entities;
using BCrypt.Net;

namespace Bestivale.Application.Services;

public sealed class AuthService : IUserService
{
    private const string RoleUser = "User";
    private const string RoleAdmin = "Admin";
    private const string RoleRootAdmin = "RootAdmin";

    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponse> RegisterUserAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var username = request.Username.Trim();

        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username is required.", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ArgumentException("Password is required.", nameof(request));
        }

        var existing = await _userRepository.GetByUsernameAsync(username, cancellationToken);
        if (existing is not null)
        {
            throw new InvalidOperationException("Username is already taken.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            Role = RoleUser,
            CurrencyBalance = 10,
            CreatedAt = DateTime.UtcNow,
            IsRootAdmin = false
        };

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        await _userRepository.AddAsync(user, cancellationToken);

        return MapToResponse(user);
    }

    public async Task<UserResponse?> LoginUserAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var username = request.Username.Trim();

        var user = await _userRepository.GetByUsernameAsync(username, cancellationToken);
        if (user is null)
        {
            return null;
        }

        var isValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!isValid)
        {
            return null;
        }

        return MapToResponse(user);
    }

    public async Task<int?> GetUserBalanceAsync(string username, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByUsernameAsync(username, cancellationToken);
        return user?.CurrencyBalance;
    }

    public async Task<IReadOnlyList<UserResponse>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        return users
            .OrderBy(u => u.Username)
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<bool> PromoteToAdminAsync(Guid userId, string performedByUsername, CancellationToken cancellationToken = default)
    {
        var performer = await RequireUserAsync(performedByUsername, cancellationToken);
        EnsureAdminRights(performer);

        var target = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (target is null || target.IsRootAdmin)
        {
            return false;
        }

        if (target.Role == RoleAdmin)
        {
            return true;
        }

        target.Role = RoleAdmin;
        await _userRepository.UpdateAsync(target, cancellationToken);
        return true;
    }

    public async Task<bool> DemoteAdminAsync(Guid userId, string performedByUsername, CancellationToken cancellationToken = default)
    {
        var performer = await RequireUserAsync(performedByUsername, cancellationToken);
        EnsureAdminRights(performer);

        var target = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (target is null || target.IsRootAdmin)
        {
            return false;
        }

        if (target.Role == RoleUser)
        {
            return true;
        }

        target.Role = RoleUser;
        await _userRepository.UpdateAsync(target, cancellationToken);
        return true;
    }

    public async Task<bool> DeleteUserAsync(Guid userId, string performedByUsername, CancellationToken cancellationToken = default)
    {
        var performer = await RequireUserAsync(performedByUsername, cancellationToken);
        EnsureAdminRights(performer);

        var target = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (target is null || target.IsRootAdmin)
        {
            return false;
        }

        await _userRepository.DeleteAsync(target, cancellationToken);
        return true;
    }

    private async Task<User> RequireUserAsync(string username, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUsernameAsync(username, cancellationToken);
        if (user is null)
        {
            throw new InvalidOperationException("Performing user does not exist.");
        }

        return user;
    }

    private static void EnsureAdminRights(User user)
    {
        if (user.Role != RoleAdmin && user.Role != RoleRootAdmin)
        {
            throw new UnauthorizedAccessException("Only Admin or RootAdmin can perform this action.");
        }
    }

    private static UserResponse MapToResponse(User user) =>
        new()
        {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role,
            CurrencyBalance = user.CurrencyBalance,
            CreatedAt = user.CreatedAt
        };
}

