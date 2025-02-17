using Microsoft.AspNetCore.Mvc;
using Eve.Api.Controllers.Common;
using Eve.Application.InternalServices.TokenService;

namespace Eve.Api.Controllers;

[Route("callback")]
public class CallbackController : BaseController
{
    private readonly IEsiTokenService _tokenService;
    private readonly IConfiguration _config;

    public CallbackController(
        IEsiTokenService tokenService,
        IConfiguration configuration)
    {
        _tokenService = tokenService;
        _config = configuration;
    }

    [HttpGet]
    public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state, CancellationToken token)
    {

        var result = await _tokenService.GetAccessTokenAsync(new RequestData(code, state), token);

        if (result.IsFailure)
            return StatusCode((int)result.Error.ErrorCode, result.Error.Message);

        Console.WriteLine($" AcessToken : {result.Value.Token}\n" +
            $"RedirectUrl : {result.Value.RedirectUrl}");

        return Redirect(result.Value.RedirectUrl);
    }
}
