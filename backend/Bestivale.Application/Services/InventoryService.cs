using Bestivale.Application.Dtos;
using Bestivale.Application.Interfaces;

namespace Bestivale.Application.Services
{
    public sealed class InventoryService
    {
        private readonly IEggRepository _eggRepository;
        private readonly IUserRepository _userRepository;

        public InventoryService(
            IEggRepository eggRepository,
            IUserRepository userRepository)
        {
            _eggRepository = eggRepository;
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