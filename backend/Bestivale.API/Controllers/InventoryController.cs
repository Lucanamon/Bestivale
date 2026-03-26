using Bestivale.Application.Dtos;
using Bestivale.Application.Services;
using Bestivale.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Bestivale.API.Controllers;

[ApiController]
[Route("api/inventory")]
public sealed class InventoryController : ControllerBase
{
    private readonly EggService _eggService;
    private readonly InventoryService _inventoryService;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IUserRepository _userRepository;

    public InventoryController(
        EggService eggService,
        InventoryService inventoryService,
        IInventoryRepository inventoryRepository,
        IUserRepository userRepository)
    {
        _eggService = eggService;
        _inventoryService = inventoryService;
        _inventoryRepository = inventoryRepository;
        _userRepository = userRepository;
    }

    [HttpGet("eggs")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<EggDto>>> GetEggs(CancellationToken cancellationToken)
    {
        var username = this.GetUsername();
        if (string.IsNullOrWhiteSpace(username))
        {
            return this.MissingUsername();
        }

        var eggs = await _eggService.GetEggsForUserAsync(username, cancellationToken);
        return Ok(eggs);
    }

    [HttpPost("favorite/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ToggleFavorite(Guid id, CancellationToken cancellationToken)
    {
        var username = this.GetUsername();
        if (string.IsNullOrWhiteSpace(username))
        {
            return this.MissingUsername();
        }

        try
        {
            await _inventoryService.ToggleFavoriteAsync(username, id, cancellationToken);
            return NoContent();
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("favorites")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<EggDto>>> GetFavorites(CancellationToken cancellationToken)
    {
        var username = this.GetUsername();
        if (string.IsNullOrWhiteSpace(username))
        {
            return this.MissingUsername();
        }

        var favorites = await _inventoryService.GetFavoritesAsync(username, cancellationToken);
        return Ok(favorites);
    }

    [HttpGet("items")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<InventoryItemDto>>> GetItems(
        [FromQuery] bool favoritesOnly = false,
        CancellationToken cancellationToken = default)
    {
        var username = this.GetUsername();
        if (string.IsNullOrWhiteSpace(username))
        {
            return this.MissingUsername();
        }

        var user = await _userRepository.GetByUsernameAsync(username.Trim(), cancellationToken);
        if (user is null)
        {
            return Ok(Array.Empty<InventoryItemDto>());
        }

        // v1: return egg inventory items from new tables; monsters will be added with admin grant endpoint.
        var eggItems = await _inventoryRepository.GetEggItemsByOwnerAsync(user.Id, favoritesOnly, cancellationToken);
        var dtos = eggItems.Select(x => new InventoryItemDto
            {
                Id = x.Item.Id,
                ItemType = "Egg",
                IsFavorite = x.Item.IsFavorite,
                IsListed = x.Item.IsListed,
                CreatedAt = x.Item.CreatedAt,
                TemplateCode = x.Egg.TemplateCode,
                ColorHex = x.Egg.ColorHex,
                ColorDescription = x.Egg.ColorDescription
            })
            .ToList();

        return Ok(dtos);
    }
}

