using Eve.Domain.Interfaces.CacheProviders;
using Eve.Infrastructure.ExternalServices;
using Eve.Infrastructure.ExternalServices.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace Eve.Tests.UnitTests.InfrastructureTests.ExternalServicesTests.EveApiCleintProvider;
public class EveApiClientProviderFactory
{
    private readonly Mock<IRedisProvider> _redisMock;
    private readonly Mock<ILogger<EveApiClientProvider>> _loggerMock;
    private readonly Mock<IEveGlobalRateLimit> _rateLimitMock;
    private readonly Mock<HttpMessageHandler> _httpHandlerMock;
    private readonly IConfiguration _config;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly HttpClient _httpClient;

    public Mock<HttpMessageHandler> HttpHandlerMock => _httpHandlerMock;

    public HttpClient HttpClient => _httpClient;

    public Mock<ILogger<EveApiClientProvider>> LoggerMock => _loggerMock;

    public Mock<IEveGlobalRateLimit> RateLimitMock => _rateLimitMock;

    public EveApiClientProviderFactory()
    {
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
        _redisMock = new Mock<IRedisProvider>();
        _loggerMock = new Mock<ILogger<EveApiClientProvider>>();
        _rateLimitMock = new Mock<IEveGlobalRateLimit>();

        var inMemorySettings = new Dictionary<string, string>
        {
            { "ESI:ClientId", "test-client-id" },
            { "ESI:ClientSecret", "test-client-secret" }
        };


        _config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _httpHandlerMock = new Mock<HttpMessageHandler>();

        _httpClient = new HttpClient(_httpHandlerMock.Object)
        {
            BaseAddress = new Uri("https://esi.evetech.net/latest/")
        };
    }

    public EveApiClientProvider GetProviderWithSuccesStatusCode<T>(T objects)
    {
        var jsonResponse = JsonSerializer.Serialize(objects, _serializerOptions);

        _redisMock.Setup(r => r.GetAsync<string>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync((string)null);

        HttpHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });

        RateLimitMock
            .Setup(r => r.RunTaskAsync(
                It.IsAny<Func<Task<HttpResponseMessage>>>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .Returns<Func<Task<HttpResponseMessage>>, bool, CancellationToken>((task, flag, token) => task());

        return new EveApiClientProvider(
            HttpClient,
            _redisMock.Object,
            LoggerMock.Object,
            _config,
            RateLimitMock.Object);
    }

    public EveApiClientProvider GetProviderWithManyPages<T>(List<T> objects)
    {
        var jsonResponse = JsonSerializer.Serialize(objects, _serializerOptions);

        _redisMock.Setup(r => r.GetAsync<string>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync((string)null);

        HttpHandlerMock
        .Protected()
        .Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>())
        .ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
        {
            var uri = request.RequestUri?.ToString();
            int page = 0;
            if (uri.Contains("page="))
            {
                page = int.Parse(uri.Split("page=")[1].Split('&')[0]);
            }

            if (page <= 100)
            {
                var jsonResponse = $"[{JsonSerializer.Serialize(objects[page-1], _serializerOptions)}]";
                Console.WriteLine(jsonResponse);
                var response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                };

                response.Headers.Add("X-Pages", "100");
                return response;
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound);
        });

        RateLimitMock
            .Setup(r => r.RunTaskAsync(
                It.IsAny<Func<Task<HttpResponseMessage>>>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .Returns<Func<Task<HttpResponseMessage>>, bool, CancellationToken>((task, flag, token) => task());

        return new EveApiClientProvider(
            HttpClient,
            _redisMock.Object,
            LoggerMock.Object,
            _config,
            RateLimitMock.Object);
    }

    public EveApiClientProvider GetProviderWithNotModifiedStatusCode()
    {
        _redisMock.Setup(r => r.GetAsync<string>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync((string)null);

        HttpHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotModified,
            });

        RateLimitMock
            .Setup(r => r.RunTaskAsync(
                It.IsAny<Func<Task<HttpResponseMessage>>>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .Returns<Func<Task<HttpResponseMessage>>, bool, CancellationToken>((task, flag, token) => task());

        return new EveApiClientProvider(
            HttpClient,
            _redisMock.Object,
            LoggerMock.Object,
            _config,
            RateLimitMock.Object);
    }

    public EveApiClientProvider GetProviderWithEmptyContent()
    {
        _redisMock.Setup(r => r.GetAsync<string>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync((string)null);

        HttpHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotModified,
                Content = new StringContent("[]")
            });

        RateLimitMock
            .Setup(r => r.RunTaskAsync(
                It.IsAny<Func<Task<HttpResponseMessage>>>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .Returns<Func<Task<HttpResponseMessage>>, bool, CancellationToken>((task, flag, token) => task());

        return new EveApiClientProvider(
            HttpClient,
            _redisMock.Object,
            LoggerMock.Object,
            _config,
            RateLimitMock.Object);
    }

    public EveApiClientProvider GetProviderWithNotSuccessStatusCode()
    {
        _redisMock.Setup(r => r.GetAsync<string>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync((string)null);

        HttpHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
            });

        RateLimitMock
            .Setup(r => r.RunTaskAsync(
                It.IsAny<Func<Task<HttpResponseMessage>>>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .Returns<Func<Task<HttpResponseMessage>>, bool, CancellationToken>((task, flag, token) => task());

        return new EveApiClientProvider(
            HttpClient,
            _redisMock.Object,
            LoggerMock.Object,
            _config,
            RateLimitMock.Object);
    }

    public EveApiClientProvider GetProviderWithSendAsyncThrowHttpRequestException(string errorMessage)
    {
        _redisMock.Setup(r => r.GetAsync<string>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync((string)null);

        HttpHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException(errorMessage));

        RateLimitMock
            .Setup(r => r.RunTaskAsync(
                It.IsAny<Func<Task<HttpResponseMessage>>>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .Returns<Func<Task<HttpResponseMessage>>, bool, CancellationToken>((task, flag, token) => task());

        return new EveApiClientProvider(
            HttpClient,
            _redisMock.Object,
            LoggerMock.Object,
            _config,
            RateLimitMock.Object);
    }

    public EveApiClientProvider GetProviderWithSendAsyncThrowAnyException(string errorMessage)
    {
        _redisMock.Setup(r => r.GetAsync<string>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync((string)null);

        HttpHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new Exception(errorMessage));

        RateLimitMock
            .Setup(r => r.RunTaskAsync(
                It.IsAny<Func<Task<HttpResponseMessage>>>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .Returns<Func<Task<HttpResponseMessage>>, bool, CancellationToken>((task, flag, token) => task());

        return new EveApiClientProvider(
            HttpClient,
            _redisMock.Object,
            LoggerMock.Object,
            _config,
            RateLimitMock.Object);
    }
}