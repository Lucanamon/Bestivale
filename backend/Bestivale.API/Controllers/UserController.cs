using Bestivale.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Bestivale.API.Controllers;

[ApiController]
[Route("api/user")]
public sealed class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("balance/{username}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<object>> GetBalance(string username, CancellationToken cancellationToken)
    {
        var balance = await _userService.GetUserBalanceAsync(username, cancellationToken);
        if (balance is null)
        {
            return NotFound();
        }

        return Ok(new { balance });
    }
}

