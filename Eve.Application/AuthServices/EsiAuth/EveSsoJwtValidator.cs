using Eve.Domain.Common;
using Eve.Domain.Constants;
using Eve.Domain.ExternalTypes;
using Eve.Domain.Interfaces.CacheProviders;
using Eve.Domain.Interfaces.ExternalServices;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Eve.Application.AuthServices.EsiAuth;
public class EveSsoJwtValidator
{
    private readonly IRedisProvider _redisProvider;
    private readonly IEveApiAuthClientProvider _apiClient;
    private readonly IConfiguration _config;

    public EveSsoJwtValidator(
        IRedisProvider redisProvider,
        IEveApiAuthClientProvider apiClient, 
        IConfiguration config)
    {
        _redisProvider = redisProvider;
        _apiClient = apiClient;
        _config = config;
    }
    public async Task<bool> IsTokenValidAsync(string jwtToken, CancellationToken token)
    {
        var clientId = _config["ESI:ClientId"];
        try
        {
            var principal = await ValidateJwtTokenAsync(jwtToken, token);
            if (principal.IsFailure)
            {
                return false;
            }

            var audienceClaim = principal.Value.FindFirst("aud")?.Value;
            return audienceClaim != null && audienceClaim.Split(' ').Contains(clientId);
        }
        catch
        {
            return false;
        }
    }

    private async Task<Result<JwksMetadata>> FetchJwksMetadataAsync(CancellationToken token)
    {
        var key = GlobalKeysCacheConstants.JwksMetadata;

        var metadata = await _redisProvider.GetOrSetAsync(key,
            () => _apiClient.FetchJwksMetadataAsync(token),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddSeconds(EveConstants.MetadataCacheTime)
            },
            token);

        if (metadata.IsSuccess)
            return metadata.Value;
        return metadata.Error;
    }

    private async Task<Result<ClaimsPrincipal>> ValidateJwtTokenAsync(string JwtToken, CancellationToken token)
    {
        var jwksMetadataResult = await FetchJwksMetadataAsync(token);

        if (jwksMetadataResult.IsFailure)
            return jwksMetadataResult.Error;

        var jwksMetadata = jwksMetadataResult.Value;

        var keys = jwksMetadata.Keys
            .Select(k => new JsonWebKey
            {
                Kty = k.Kty,
                Alg = k.Alg,
                Kid = k.Kid,
                E = k.E,
                N = k.N,
                Use = k.Use
            })
            .ToList();

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuers = EveConstants.AcceptedIssuers,
            ValidateAudience = true,
            ValidAudience = EveConstants.ExpectedAudience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = keys,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero 
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var principal = tokenHandler.ValidateToken(JwtToken, validationParameters, out _);
            return principal;
        }
        catch (SecurityTokenException)
        {
            return Error.InternalServer("Token not valid");
        }
    }

}
