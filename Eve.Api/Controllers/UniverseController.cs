using Eve.Api.Controllers.Common;
using Microsoft.AspNetCore.Mvc;

namespace Eve.Api.Controllers;

public class UniverseController : BaseController
{

    [HttpGet("regions/all")]
    public async Task<IActionResult> GetAllRegions(CancellationToken token)
    {
        return Ok("test");
    }

    [HttpGet("regions/{systemId}")]
    public async Task<IActionResult> GetRegionBySystemId(int systemId, CancellationToken token)
    {
        return Ok("test");
    }

    [HttpGet("regions/search/{regionName}")]
    public async Task<IActionResult> GetRegionByRegionName(string regionName, CancellationToken token)
    {
        return Ok("test");
    }

    [HttpGet("systems/{locationId}")]
    public async Task<IActionResult> GetSystemInfoByLocationId(int locationId, CancellationToken token)
    {
        return Ok("test");
    }

    [HttpGet("systems/all/{regionId}")]
    public async Task<IActionResult> GetSystemForRegionId(int regionId, CancellationToken token)
    {
        return Ok("test");
    }
}
