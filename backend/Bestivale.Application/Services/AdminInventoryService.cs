using Bestivale.Application.Dtos;
using Bestivale.Application.Interfaces;
using Bestivale.Domain.Entities;

namespace Bestivale.Application.Services;

public sealed class AdminInventoryService
{
    private readonly IUserRepository _userRepository;
    private readonly IMonsterRepository _monsterRepository;
    private readonly IInventoryRepository _inventoryRepository;

    public AdminInventoryService(
        IUserRepository userRepository,
        IMonsterRepository monsterRepository,
        IInventoryRepository inventoryRepository)
    {
        _userRepository = userRepository;
        _monsterRepository = monsterRepository;
        _inventoryRepository = inventoryRepository;
    }

    public async Task GrantMonsterAsync(string actingAdminUsername, GrantMonsterRequest request, CancellationToken cancellationToken = default)
    {
        var acting = await _userRepository.GetByUsernameAsync(actingAdminUsername.Trim(), cancellationToken);
        if (acting is null)
        {
            throw new InvalidOperationException("Acting admin does not exist.");
        }

        if (acting.Role is not ("Admin" or "RootAdmin"))
        {
            throw new UnauthorizedAccessException("Only Admin or RootAdmin can grant monsters.");
        }

        var target = await _userRepository.GetByUsernameAsync(request.TargetUsername.Trim(), cancellationToken);
        if (target is null)
        {
            throw new InvalidOperationException("Target user does not exist.");
        }

        var monsters = await _monsterRepository.GetAllAsync(cancellationToken);
        var monster = monsters.FirstOrDefault(m => m.Id == request.MonsterId);
        if (monster is null)
        {
            throw new InvalidOperationException("Monster does not exist.");
        }

        await _inventoryRepository.AddMonsterItemAsync(target.Id, monster.Id, cancellationToken);
    }
}

