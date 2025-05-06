using Eve.Application.AuthServices.Cryptography;
using Eve.Application.DTOs.TokensData;
using Eve.Domain.Common;
using Eve.Domain.Constants;
using Eve.Domain.ExternalTypes;
using Eve.Domain.Interfaces.CacheProviders;
using Eve.Domain.Interfaces.ExternalServices;
using Microsoft.Extensions.Configuration;

namespace Eve.Application.AuthServices.EsiAuth;
public class EsiAuthentication
{
    private readonly IConfiguration _config;
    private readonly IEveApiAuthClientProvider _eveApiClient;
    private readonly IRedisProvider _redisProvider;
    private readonly EncryptionService _encryption;
    private readonly EveSsoJwtValidator _jwtValidator;

    public EsiAuthentication(
        IConfiguration config,
        EncryptionService encryption,
        IRedisProvider redisProvider,
        IEveApiAuthClientProvider eveApiClient,
        EveSsoJwtValidator jwtValidator)
    {
        _config = config;
        _encryption = encryption;
        _redisProvider = redisProvider;
        _eveApiClient = eveApiClient;
        _jwtValidator = jwtValidator;
    }

    public Result<string> GetAuthUrl(string returnUrl, string stateStr)
    {
        var redirectUri = _config["ESI:RedirectUrl"];
        var clientId = _config["ESI:ClientId"];

        var state = new EsiState(stateStr, returnUrl);

        var encryptedState = _encryption.Encrypt(state);
        var safeState = Uri.EscapeDataString(encryptedState);

        var authUrl = $"https://login.eveonline.com/v2/oauth/authorize" +
                      $"?response_type=code&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                      $"&client_id={clientId}&scope={Uri.EscapeDataString(EsiScopes.Universe_ReadStructure)}" +
                      $"&state={safeState}";
        Console.WriteLine(encryptedState);
        Console.WriteLine(safeState);
        return authUrl;
    }

    public async Task<Result<string>> GetAccessTokenAsync(RequestData data, CancellationToken token)
    {
        var adminState = _config["ESI:AdminState"];
        var userState = _config["ESI:UserState"];

        if (string.IsNullOrWhiteSpace(data.Code))
            return Error.BadRequest("Authorization code is missing");

        if (string.IsNullOrWhiteSpace(data.State))
            return Error.BadRequest("invalid authorization");
        Console.WriteLine(data.State);
        var encryptedState = Uri.UnescapeDataString(data.State);
        Console.WriteLine(encryptedState);
        var state = _encryption.Decrypt<EsiState>(encryptedState);

        if (state.State.Equals(adminState))
        {
            var result = await ProcessAdminToken(state, data.Code, token);

            return result.IsFailure? result.Error : state.returnUrl;
        }
        if (state.State.Equals(userState))
        {
            var result = await ProcessUserToken(state, data.Code, token);

            return result.IsFailure ? result.Error : state.returnUrl;
        }
        return Error.BadRequest("Invalid response from server state is not valid.");
    }

    private async Task<Result> ProcessUserToken(EsiState state, string code, CancellationToken token)
    {
        var tokenResponse = await ExchangeTokenData(code, token);

        if (tokenResponse.IsFailure)
            return tokenResponse.Error;

        return Result.Success();
    }

    private async Task<Result> ProcessAdminToken(EsiState state, string code, CancellationToken token)
    {

        var tokenResponse = await ExchangeTokenData(code, token);

        if (tokenResponse.IsFailure)
            return tokenResponse.Error;

        await SaveTokensAsync(
                GlobalKeysCacheConstants.AdminTokenData,
                tokenResponse.Value,
                token);

        return Result.Success();
    }

    private async Task<Result<TokenResponse>> ExchangeTokenData(string code, CancellationToken token)
    {
        var tokenResponse = await _eveApiClient.ExchangeCodeForTokenAsync(code, token);
        if (tokenResponse.IsSuccess
            && await _jwtValidator.IsTokenValidAsync(
                tokenResponse.Value.AccessToken,
                token))
        {
            return tokenResponse.Value;
        }

        return Error.Unauthorized("Token is not valid");

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

public record EsiState(string State, string returnUrl);
