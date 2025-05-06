using Eve.Api.Controllers.Common;
using Eve.Application.AuthServices.EsiAuth;
using Eve.Application.InternalServices;
using Eve.Application.StaticDataLoaders;
using Eve.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Eve.Api.Controllers;
[Authorize]
[Route("admin")]
public class AdminController : BaseController
{
    private readonly IConfiguration _config;
    private readonly EsiAuthentication _esiAuth;
    public AdminController(IConfiguration config, EsiAuthentication esiAuth)
    {
        _config = config;
        _esiAuth = esiAuth;
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

    [HttpGet("authorizebot")]
    public async Task<IActionResult> AuthorizeBot()
    {

        var redirectUri = _config["ESI:RedirectUrl"]; 
        var clientId = _config["ESI:ClientId"];
        var adminState = _config["ESI:AdminState"];

        var result = _esiAuth.GetAuthUrl("http://localhost:3000/admin", adminState);

        if (result.IsFailure)
        {
            return BadRequest();
        }

        return Ok(new { url = result.Value });
    }
}