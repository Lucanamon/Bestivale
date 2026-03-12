using Bestivale.Application.Dtos;
using Bestivale.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Bestivale.API.Controllers;

[ApiController]
[Route("api/market")]
public sealed class MarketController : ControllerBase
{
    private readonly IMarketService _marketService;

    public MarketController(IMarketService marketService)
    {
        _marketService = marketService;
    }

    private string? GetActingUsername()
        => Request.Headers["X-Username"].FirstOrDefault();

    [HttpGet("listings")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<MarketListingDto>>> GetActiveListings(CancellationToken cancellationToken)
    {
        var listings = await _marketService.GetActiveListingsAsync(cancellationToken);
        return Ok(listings);
    }

    [HttpPost("listings")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<MarketListingDto>> CreateListing([FromBody] CreateMarketListingRequest request, CancellationToken cancellationToken)
    {
        var acting = GetActingUsername();
        if (string.IsNullOrWhiteSpace(acting))
        {
            return Unauthorized("Missing X-Username header.");
        }

        try
        {
            var listing = await _marketService.CreateListingAsync(acting, request, cancellationToken);
            return CreatedAtAction(nameof(GetActiveListings), new { id = listing.Id }, listing);
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("buy")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Buy([FromBody] BuyMarketListingRequest request, CancellationToken cancellationToken)
    {
        var acting = GetActingUsername();
        if (string.IsNullOrWhiteSpace(acting))
        {
            return Unauthorized("Missing X-Username header.");
        }

        try
        {
            var success = await _marketService.BuyListingAsync(acting, request, cancellationToken);
            if (!success)
            {
                return BadRequest("Listing not found or not active.");
            }
            return NoContent();
        }
        catch (Exception ex) when (ex is InvalidOperationException)
        {
            return BadRequest(ex.Message);
        }
    }
}

