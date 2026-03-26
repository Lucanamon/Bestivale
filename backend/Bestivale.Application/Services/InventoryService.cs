using Bestivale.Application.Dtos;
using Bestivale.Application.Interfaces;

namespace Bestivale.Application.Services
{
    public sealed class InventoryService
    {
        private readonly IEggRepository _eggRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IUserRepository _userRepository;

        public InventoryService(
            IEggRepository eggRepository,
            IInventoryRepository inventoryRepository,
            IUserRepository userRepository)
        {
            _eggRepository = eggRepository;
            _inventoryRepository = inventoryRepository;
            _userRepository = userRepository;
        }

        public async Task ToggleFavoriteAsync(
            string username,
            Guid itemId,
            CancellationToken cancellationToken = default)
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

            var egg = await _eggRepository.GetByIdAsync(itemId, cancellationToken);
            if (egg is null)
            {
                throw new InvalidOperationException("Inventory item not found.");
            }

            if (egg.OwnerUserId != user.Id)
            {
                throw new InvalidOperationException("You do not own this inventory item.");
            }

            egg.IsFavorite = !egg.IsFavorite;
            await _eggRepository.UpdateAsync(egg, cancellationToken);

            // Dual-write: keep new InventoryItem head in sync (if present).
            var invItem = await _inventoryRepository.GetItemByIdAsync(itemId, cancellationToken);
            if (invItem is not null)
            {
                invItem.IsFavorite = egg.IsFavorite;
                await _inventoryRepository.UpdateItemAsync(invItem, cancellationToken);
            }
        }

        public async Task<IReadOnlyList<EggDto>> GetFavoritesAsync(
            string username,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username is required.", nameof(username));
            }

            var user = await _userRepository.GetByUsernameAsync(username.Trim(), cancellationToken);
            if (user is null)
            {
                return Array.Empty<EggDto>();
            }

            // Prefer new inventory tables if populated; fall back to legacy eggs.
            var invEggs = await _inventoryRepository.GetEggItemsByOwnerAsync(user.Id, favoritesOnly: true, cancellationToken);
            if (invEggs.Count > 0)
            {
                return invEggs.Select(x => new EggDto
                    {
                        Id = x.Item.Id,
                        TemplateCode = x.Egg.TemplateCode,
                        ColorHex = x.Egg.ColorHex,
                        ColorDescription = x.Egg.ColorDescription,
                        CreatedAt = x.Egg.CreatedAt,
                        IsFavorite = x.Item.IsFavorite
                    })
                    .ToList();
            }

            var eggs = await _eggRepository.GetFavoritesByOwnerAsync(user.Id, cancellationToken);
            return eggs
                .Where(e => !e.IsListed)
                .Select(e => new EggDto
                {
                    Id = e.Id,
                    TemplateCode = e.TemplateCode,
                    ColorHex = e.ColorHex,
                    ColorDescription = e.ColorDescription,
                    CreatedAt = e.CreatedAt,
                    IsFavorite = e.IsFavorite
                })
                .ToList();
        }
    }
}