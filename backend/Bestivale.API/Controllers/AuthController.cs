using Bestivale.Application.Dtos;
using Bestivale.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Bestivale.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserResponse>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userService.RegisterUserAsync(request, cancellationToken);
            return CreatedAtAction(nameof(Register), new { id = user.Id }, user);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _userService.LoginUserAsync(request, cancellationToken);
        if (user is null)
        {
            return Unauthorized();
        }

        // In a real system you'd issue a JWT or similar token here.
        return Ok(user);
    }
}

