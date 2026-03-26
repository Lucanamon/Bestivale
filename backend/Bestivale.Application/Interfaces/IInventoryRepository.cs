using Bestivale.Domain.Entities;

namespace Bestivale.Application.Interfaces;

public interface IInventoryRepository
{
    Task<InventoryItem?> GetItemByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<InventoryEgg?> GetEggByItemIdAsync(Guid inventoryItemId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<(InventoryItem Item, InventoryEgg Egg)>> GetEggItemsByOwnerAsync(
        Guid ownerUserId,
        bool favoritesOnly,
        CancellationToken cancellationToken = default);

    Task UpsertEggItemFromLegacyEggAsync(Egg egg, CancellationToken cancellationToken = default);

    Task<Guid> AddMonsterItemAsync(Guid ownerUserId, Guid monsterId, CancellationToken cancellationToken = default);

    Task UpdateItemAsync(InventoryItem item, CancellationToken cancellationToken = default);
}

