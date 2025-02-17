using Eve.Domain.Common;
using Eve.Domain.Constants;
using Eve.Domain.ExternalTypes;
using Eve.Domain.Interfaces.CacheProviders;
using Eve.Domain.Interfaces.ExternalServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Eve.Application.DTOs.TokensData;

namespace Eve.Application.InternalServices.TokenService;
public class EsiTokenService : IEsiTokenService
{
    private readonly IEveApiAuthClientProvider _eveApiClient;
    private readonly IConfiguration _config;
    private readonly IRedisProvider _redisProvider;
    private readonly EveSsoJwtValidator _jwtValidator;
    private readonly ILogger<EsiTokenService> _logger;

    public EsiTokenService(
        IEveApiAuthClientProvider eveApiClient,
        IConfiguration config,
        IRedisProvider redisProvider,
        ILogger<EsiTokenService> logger,
        EveSsoJwtValidator jwtValidator)
    {
        _eveApiClient = eveApiClient;
        _config = config;
        _redisProvider = redisProvider; 
        _logger = logger;
        _jwtValidator = jwtValidator;
    }


    public async Task<Result<ResponseTokenData>> GetAccessTokenAsync(RequestData data, CancellationToken token)
    {
        var adminState = _config["ESI:AdminState"];

        var response = new ResponseTokenData();

        if (string.IsNullOrWhiteSpace(data.Code))
            return Error.BadRequest("Authorization code is missing");

        if (string.IsNullOrWhiteSpace(data.State))
            return Error.BadRequest("invalid avtorization");

        var tokenResponse = await _eveApiClient.ExchangeCodeForTokenAsync(data.Code, token);
        if (tokenResponse.IsFailure)
            return tokenResponse.Error;

        if (data.State.Equals(adminState))
        {
            await SaveTokensAsync(
                GlobalKeysCacheConstants.AdminTokenData,
                tokenResponse.Value, 
                token);
            response.RedirectUrl = "http://localhost:3000/admin";
            response.Token = tokenResponse.Value.AccessToken;

            return response;
        }
        if (!data.State.StartsWith("http://localhost:3000"))
            return Error.BadRequest("invalid avtorization");
        response.Token = tokenResponse.Value.AccessToken;
        response.RedirectUrl = data.State;

        return response;
    }

    private async Task SaveTokensAsync(string key, TokenResponse tokenResponse, CancellationToken token)
    {
        var tokenData = new EveApiTokenData
        {
            AccessToken = tokenResponse.AccessToken,
            TokenType = tokenResponse.TokenType,
            ExpiresIn = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
            RefreshToken = tokenResponse.RefreshToken,
            UpdatedAt = DateTime.UtcNow,
        };

        await _redisProvider.SetAsync(key, tokenData, token);
    }
}
