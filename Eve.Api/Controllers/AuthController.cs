using Eve.Api.Controllers.Common;
using Eve.Application.AuthServices.AuthTokenService;
using Eve.Application.AuthServices.EsiAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Eve.Api.Controllers;

public class AuthController : BaseController
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly EsiAuthentication _esiAuthentication;
    private readonly IConfiguration _config;

    public AuthController(
        UserManager<IdentityUser> userManager,
        ITokenService tokenService,
        EsiAuthentication esiAuthentication,
        IConfiguration config)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _esiAuthentication = esiAuthentication;
        _config = config;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken token)
    {
        var user = await _userManager.FindByNameAsync(request.UserName);
        if (user == null)
            return Unauthorized("invalid user name ");

        if(!await _userManager.CheckPasswordAsync(user, request.Password))
            return Unauthorized("invalid user password ");

        var jwtToken = _tokenService.GenerateJwtToken(user);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Unspecified,
            Expires = DateTime.UtcNow.AddDays(1)
        };

        Response.Cookies.Append("AuthToken", jwtToken, cookieOptions);
        return Ok(new 
        {
            message = "Login success"
        });
    }

    [HttpGet("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken token)
    {
        Response.Cookies.Delete("AuthToken");
        return Ok(new { message = "Successfully logged out" });
    }

    [HttpGet("/callback")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state, CancellationToken token)
    {
        var result = await _esiAuthentication.GetAccessTokenAsync(new RequestData(code, state), token);

        if (result.IsFailure)
            return StatusCode((int)result.Error.ErrorCode, result.Error.Message);


        return Redirect(result.Value);
    }

    [HttpGet("esi/login")]
    public async Task<IActionResult> AuthorizeBot(string returnUrl)
    {
        var state = _config["ESI:UserState"];
        var result = _esiAuthentication.GetAuthUrl(returnUrl, state);

        if (result.IsFailure)
            return StatusCode((int)result.Error.ErrorCode, result.Error.Message);


        return Redirect(result.Value);
    }
}

public class LoginRequest
{
    public string UserName { get; set; }
    public string Password { get; set; }    
}