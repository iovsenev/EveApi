using Eve.Domain.Common;
using Eve.Domain.ExternalTypes;
using Eve.Domain.Interfaces.CacheProviders;
using Eve.Infrastructure.ExternalServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Npgsql.Internal;
using System.Net;
using System.Text.Json;

namespace Eve.Tests.UnitTests.InfrastructureTests.ExternalServicesTests.EveApiCleintProvider;

public class FetchJwksMetadataAsyncTests
{
    private JsonSerializerOptions _serializerOptions;
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly Mock<IRedisProvider> _redisProviderMock;
    private readonly Mock<ILogger<EveApiClientProvider>> _loggerMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly Mock<IEveGlobalRateLimit> _globalRateLimitMock;
    private readonly EveApiClientProvider _eveApiClientProvider;

    public FetchJwksMetadataAsyncTests()
    {
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _redisProviderMock = new Mock<IRedisProvider>();
        _loggerMock = new Mock<ILogger<EveApiClientProvider>>();
        _configMock = new Mock<IConfiguration>();
        _globalRateLimitMock = new Mock<IEveGlobalRateLimit>();

        _configMock.Setup(c => c["ESI:ClientId"]).Returns("clientId");
        _configMock.Setup(c => c["ESI:ClientSecret"]).Returns("clientSecret");

        _eveApiClientProvider = new EveApiClientProvider(
            _httpClient,
            _redisProviderMock.Object,
            _loggerMock.Object,
            _configMock.Object,
            _globalRateLimitMock.Object
        );
    }

    [Fact]
    public async Task FetchJwksMetadataAsync_ReturnJwksMetadata_WhenRequestsAreSuccessful()
    {
        // Arrange
        var metadataResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(new MetadataResponse
            {
                JwksUri = "https://example.com/jwks"
            },_serializerOptions)),
        };

        var jwksResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(new JwksMetadata
            {
                Keys = new List<JwksKey> { new JwksKey { Kid = "1", Alg = "RS256" } }
            }, _serializerOptions)),
        };

        _httpMessageHandlerMock
            .Protected()
            .SetupSequence<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(metadataResponse)
            .ReturnsAsync(jwksResponse);

        _globalRateLimitMock
            .Setup(r => r.RunTaskAsync(
                It.IsAny<Func<Task<HttpResponseMessage>>>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .Returns<Func<Task<HttpResponseMessage>>, bool, CancellationToken>((task, flag, token) => task());

        // Act
        var result = await _eveApiClientProvider.FetchJwksMetadataAsync(CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value.Keys);
    }

    [Fact]
    public async Task FetchJwksMetadataAsync_ReturnError_WhenMetadataRequestFails()
    {
        // Arrange
        var metadataResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.InternalServerError
        };

        _globalRateLimitMock
            .Setup(r => r.RunTaskAsync(
                It.IsAny<Func<Task<HttpResponseMessage>>>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .Returns<Func<Task<HttpResponseMessage>>, bool, CancellationToken>((task, flag, token) => task());

       

        // Act
        var result = await _eveApiClientProvider.FetchJwksMetadataAsync(CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodes.InternalServer, result.Error.ErrorCode);
    }

    [Fact]
    public async Task FetchJwksMetadataAsync_ReturnError_WhenJwksRequestFails()
    {
        // Arrange
        var metadataResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(new MetadataResponse
            {
                JwksUri = "https://example.com/jwks"
            }))
        };

        var jwksResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.InternalServerError
        };

        _globalRateLimitMock
            .Setup(r => r.RunTaskAsync(
                It.IsAny<Func<Task<HttpResponseMessage>>>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .Returns<Func<Task<HttpResponseMessage>>, bool, CancellationToken>((task, flag, token) => task());

        

        // Act
        var result = await _eveApiClientProvider.FetchJwksMetadataAsync(CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodes.InternalServer, result.Error.ErrorCode);
    }
}
