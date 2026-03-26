using Bestivale.Application.Interfaces;
using Bestivale.Domain.Entities;
using Bestivale.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Bestivale.Infrastructure.Repositories;

public sealed class InventoryRepository : IInventoryRepository
{
    private readonly BestivaleDbContext _db;

    public InventoryRepository(BestivaleDbContext db)
    {
        _db = db;
    }

    public async Task<InventoryItem?> GetItemByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _db.InventoryItems.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<InventoryEgg?> GetEggByItemIdAsync(Guid inventoryItemId, CancellationToken cancellationToken = default)
    {
        return await _db.InventoryEggs.FirstOrDefaultAsync(e => e.InventoryItemId == inventoryItemId, cancellationToken);
    }

    public async Task<IReadOnlyList<(InventoryItem Item, InventoryEgg Egg)>> GetEggItemsByOwnerAsync(
        Guid ownerUserId,
        bool favoritesOnly,
        CancellationToken cancellationToken = default)
    {
        var query =
            from item in _db.InventoryItems.AsNoTracking()
            join egg in _db.InventoryEggs.AsNoTracking() on item.Id equals egg.InventoryItemId
            where item.OwnerUserId == ownerUserId && item.ItemType == InventoryItemType.Egg
            select new { item, egg };

        if (favoritesOnly)
        {
            query = query.Where(x => x.item.IsFavorite);
        }

        // Hide listed items from inventory views (consistent with legacy behaviour).
        query = query.Where(x => !x.item.IsListed).OrderBy(x => x.egg.CreatedAt);

        var rows = await query.ToListAsync(cancellationToken);
        return rows.Select(r => (r.item, r.egg)).ToList();
    }

    public async Task UpsertEggItemFromLegacyEggAsync(Egg egg, CancellationToken cancellationToken = default)
    {
        // Use the legacy Egg.Id as the InventoryItem.Id for compatibility.
        var item = await _db.InventoryItems.FirstOrDefaultAsync(i => i.Id == egg.Id, cancellationToken);
        if (item is null)
        {
            item = new InventoryItem
            {
                Id = egg.Id,
                OwnerUserId = egg.OwnerUserId,
                ItemType = InventoryItemType.Egg,
                IsFavorite = egg.IsFavorite,
                IsListed = egg.IsListed,
                CreatedAt = egg.CreatedAt
            };
            _db.InventoryItems.Add(item);
        }
        else
        {
            item.OwnerUserId = egg.OwnerUserId;
            item.ItemType = InventoryItemType.Egg;
            item.IsFavorite = egg.IsFavorite;
            item.IsListed = egg.IsListed;
            item.CreatedAt = egg.CreatedAt;
            _db.InventoryItems.Update(item);
        }

        var eggRow = await _db.InventoryEggs.FirstOrDefaultAsync(e => e.InventoryItemId == egg.Id, cancellationToken);
        if (eggRow is null)
        {
            eggRow = new InventoryEgg
            {
                InventoryItemId = egg.Id,
                TemplateCode = egg.TemplateCode,
                ColorHex = egg.ColorHex,
                ColorDescription = egg.ColorDescription,
                CreatedAt = egg.CreatedAt
            };
            _db.InventoryEggs.Add(eggRow);
        }
        else
        {
            eggRow.TemplateCode = egg.TemplateCode;
            eggRow.ColorHex = egg.ColorHex;
            eggRow.ColorDescription = egg.ColorDescription;
            eggRow.CreatedAt = egg.CreatedAt;
            _db.InventoryEggs.Update(eggRow);
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<Guid> AddMonsterItemAsync(Guid ownerUserId, Guid monsterId, CancellationToken cancellationToken = default)
    {
        var itemId = Guid.NewGuid();

        var item = new InventoryItem
        {
            Id = itemId,
            OwnerUserId = ownerUserId,
            ItemType = InventoryItemType.Monster,
            IsFavorite = false,
            IsListed = false,
            CreatedAt = DateTime.UtcNow
        };

        var monster = new InventoryMonster
        {
            InventoryItemId = itemId,
            MonsterId = monsterId
        };

        _db.InventoryItems.Add(item);
        _db.InventoryMonsters.Add(monster);
        await _db.SaveChangesAsync(cancellationToken);
        return itemId;
    }

    public async Task UpdateItemAsync(InventoryItem item, CancellationToken cancellationToken = default)
    {
        _db.InventoryItems.Update(item);
        await _db.SaveChangesAsync(cancellationToken);
    }
}

