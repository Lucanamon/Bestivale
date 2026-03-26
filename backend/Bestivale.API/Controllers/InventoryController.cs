using Bestivale.Application.Dtos;
using Bestivale.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bestivale.API.Controllers;

[ApiController]
[Route("api/inventory")]
public sealed class InventoryController : ControllerBase
{
    private readonly EggService _eggService;
    private readonly InventoryService _inventoryService;

    public InventoryController(EggService eggService, InventoryService inventoryService)
    {
        _eggService = eggService;
        _inventoryService = inventoryService;
    }

    private string? GetUsername() => Request.Headers["X-Username"].FirstOrDefault();

    [HttpGet("eggs")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<EggDto>>> GetEggs(CancellationToken cancellationToken)
    {
        var username = GetUsername();
        if (string.IsNullOrWhiteSpace(username))
        {
            return Unauthorized("Missing X-Username header.");
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
        var username = GetUsername();
        if (string.IsNullOrWhiteSpace(username))
        {
            return Unauthorized("Missing X-Username header.");
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
        var username = GetUsername();
        if (string.IsNullOrWhiteSpace(username))
        {
            return Unauthorized("Missing X-Username header.");
        }

        var favorites = await _inventoryService.GetFavoritesAsync(username, cancellationToken);
        return Ok(favorites);
    }
}

