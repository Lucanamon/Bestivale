using Bestivale.Application.Dtos;
using Bestivale.Application.Interfaces;
using Bestivale.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bestivale.API.Controllers;

[ApiController]
[Route("api/admin")]
public sealed class AdminController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly AdminInventoryService _adminInventoryService;

    public AdminController(IUserService userService, AdminInventoryService adminInventoryService)
    {
        _userService = userService;
        _adminInventoryService = adminInventoryService;
    }

    // For now, the acting admin username is taken from the X-Admin-Username header.
    // This should be wired to the authenticated user identity in a real system.
    [HttpGet("users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<UserResponse>>> GetAllUsers(CancellationToken cancellationToken)
    {
        var acting = this.GetAdminUsername();
        if (string.IsNullOrWhiteSpace(acting))
        {
            return this.MissingAdminUsername();
        }

        // Authorization is enforced inside the service based on role.
        try
        {
            var users = await _userService.GetAllUsersAsync(cancellationToken);
            return Ok(users);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [HttpPost("promote/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Promote(Guid userId, CancellationToken cancellationToken)
    {
        var acting = this.GetAdminUsername();
        if (string.IsNullOrWhiteSpace(acting))
        {
            return this.MissingAdminUsername();
        }

        try
        {
            var success = await _userService.PromoteToAdminAsync(userId, acting, cancellationToken);
            if (!success)
            {
                return BadRequest("Unable to promote user (user not found or is RootAdmin).");
            }

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [HttpPost("demote/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Demote(Guid userId, CancellationToken cancellationToken)
    {
        var acting = this.GetAdminUsername();
        if (string.IsNullOrWhiteSpace(acting))
        {
            return this.MissingAdminUsername();
        }

        try
        {
            var success = await _userService.DemoteAdminAsync(userId, acting, cancellationToken);
            if (!success)
            {
                return BadRequest("Unable to demote user (user not found or is RootAdmin).");
            }

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [HttpDelete("delete/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(Guid userId, CancellationToken cancellationToken)
    {
        var acting = this.GetAdminUsername();
        if (string.IsNullOrWhiteSpace(acting))
        {
            return this.MissingAdminUsername();
        }

        try
        {
            var success = await _userService.DeleteUserAsync(userId, acting, cancellationToken);
            if (!success)
            {
                return BadRequest("Unable to delete user (user not found or is RootAdmin).");
            }

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [HttpPost("grant/monster")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GrantMonster([FromBody] GrantMonsterRequest request, CancellationToken cancellationToken)
    {
        var acting = this.GetAdminUsername();
        if (string.IsNullOrWhiteSpace(acting))
        {
            return this.MissingAdminUsername();
        }

        try
        {
            await _adminInventoryService.GrantMonsterAsync(acting, request, cancellationToken);
            return NoContent();
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }
}

