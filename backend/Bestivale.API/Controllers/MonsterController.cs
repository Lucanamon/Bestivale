using Bestivale.Application.Dtos;
using Bestivale.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Bestivale.API.Controllers;

[ApiController]
[Route("api/monsters")]
public sealed class MonsterController : ControllerBase
{
    private readonly IMonsterService _monsterService;

    public MonsterController(IMonsterService monsterService)
    {
        _monsterService = monsterService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<MonsterDto>>> GetAll(CancellationToken cancellationToken)
    {
        var monsters = await _monsterService.GetAllAsync(cancellationToken);
        return Ok(monsters);
    }
}

