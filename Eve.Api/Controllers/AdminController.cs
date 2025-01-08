using Eve.Api.Controllers.Common;
using Eve.Application.ExternalDataLoaders;
using Eve.Application.StaticDataLoaders;
using Eve.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eve.Api.Controllers;
[Authorize]
public class AdminController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> CreateDb([FromServices] EntityLoader creater, CancellationToken token)
    {
        await creater.Run(token);

        return Ok();
    }

    [HttpGet("orders")]
    public async Task<IActionResult> LoadOrders([FromServices] IOrdersLoader loader, CancellationToken token)
    {

        Task.WaitAll(
            [
                loader.Load((int)CentralHubRegionId.Jita,token),
                loader.Load((int)CentralHubRegionId.Dodixie,token),
                loader.Load((int)CentralHubRegionId.Amarr,token),
                loader.Load((int)CentralHubRegionId.Rens, token),
            ]);

        return Ok();
    }

    [HttpGet("authTest")]
    public async Task<IActionResult> TestAuth()
    {
        return Ok(new { message = "athorize"});
    }
}