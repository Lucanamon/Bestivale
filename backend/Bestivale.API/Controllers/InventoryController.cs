using Bestivale.Application.Dtos;
using Bestivale.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bestivale.API.Controllers;

[ApiController]
[Route("api/inventory")]
public sealed class InventoryController : ControllerBase
{
    private readonly EggService _eggService;

    public InventoryController(EggService eggService)
    {
        _eggService = eggService;
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
}

