using Bestivale.Application.Dtos;
using Bestivale.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bestivale.API.Controllers;

[ApiController]
[Route("api/fate")]
public sealed class FateController : ControllerBase
{
    private readonly EggService _eggService;

    public FateController(EggService eggService)
    {
        _eggService = eggService;
    }

    private string? GetUsername() => Request.Headers["X-Username"].FirstOrDefault();

    public sealed class DrawRequest
    {
        public int Tier { get; init; }
    }

    [HttpPost("draw")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EggDto>> Draw([FromBody] DrawRequest request, CancellationToken cancellationToken)
    {
        var username = GetUsername();
        if (string.IsNullOrWhiteSpace(username))
        {
            return Unauthorized("Missing X-Username header.");
        }

        try
        {
            // For now all tiers grant a single random egg;
            // tier-specific behaviour can be added later.
            var egg = await _eggService.GrantRandomEggForUserAsync(username, cancellationToken);
            return Ok(egg);
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            return BadRequest(ex.Message);
        }
    }
}

