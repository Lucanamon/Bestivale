using Bestivale.Application.Dtos;

namespace Bestivale.Application.Interfaces;

public interface IUserService
{
    Task<UserResponse> RegisterUserAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    Task<UserResponse?> LoginUserAsync(LoginRequest request, CancellationToken cancellationToken = default);

    Task<int?> GetUserBalanceAsync(string username, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UserResponse>> GetAllUsersAsync(CancellationToken cancellationToken = default);

    Task<bool> PromoteToAdminAsync(Guid userId, string performedByUsername, CancellationToken cancellationToken = default);

    Task<bool> DemoteAdminAsync(Guid userId, string performedByUsername, CancellationToken cancellationToken = default);

    Task<bool> DeleteUserAsync(Guid userId, string performedByUsername, CancellationToken cancellationToken = default);
}

