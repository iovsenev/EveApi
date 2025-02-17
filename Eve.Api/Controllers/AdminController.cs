using Eve.Api.Controllers.Common;
using Eve.Application.InternalServices;
using Eve.Application.StaticDataLoaders;
using Eve.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Eve.Api.Controllers;
[Authorize]
public class AdminController : BaseController
{
    private readonly IConfiguration _config;

    public AdminController(IConfiguration config)
    {
        _config = config;
    }

    [HttpGet ("initialdatabase")]
    public async Task<IActionResult> CreateDb([FromServices] EntityLoader creater, CancellationToken token)
    {
        await creater.Run(token);

        return Ok();
    }

    [HttpGet("orders")]
    public async Task<IActionResult> LoadOrders([FromServices] ILoadOrdersService loader, CancellationToken token)
    {

        var result = await loader.RunTaskAsync(token);
        if (!result)
            return BadRequest(new { message = "not loaded" });

        return Ok(new { message = "loaded" });
    }

    [HttpGet("orders2")]
    public async Task<IActionResult> LoadOrders2([FromServices] ILoadOrdersService loader, CancellationToken token)
    {

        var result = await loader.RunTaskAsync(token);
        if (!result)
            return BadRequest(new { message = "not loaded" });

        return Ok(new { message = "loaded" });
    }

    [HttpGet("authTest")]
    public async Task<IActionResult> TestAuth()
    {
        var userName = User.FindFirst(JwtRegisteredClaimNames.Name)?.Value;
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return Ok(new
        {
            message = "athorize",
            userName = userName,
            userId = userId
        });
    }

    [HttpGet("start")]
    public async Task<IActionResult> AuthorizeBot()
    {

        var redirectUri = "http://localhost:5000/callback";
        var clientId = _config["ESI:ClientId"];
        var adminState = _config["ESI:AdminState"];

        var authUrl = $"https://login.eveonline.com/v2/oauth/authorize" +
                      $"?response_type=code&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                      $"&client_id={clientId}&scope={Uri.EscapeDataString(EsiScopes.Universe_ReadStructure)}" +
                      $"&state={adminState}";

        return Ok(new { url = authUrl });
    }

    
}