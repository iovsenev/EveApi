using Eve.Domain.Common;
using Eve.Domain.Constants;
using Eve.Domain.ExternalTypes;
using Eve.Domain.Interfaces.CacheProviders;
using Eve.Domain.Interfaces.ExternalServices;
using Eve.Infrastructure.ExternalServices.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Eve.Infrastructure.ExternalServices;

public class EveApiClientProvider : IEveApiOpenClientProvider, IEveApiAuthClientProvider
{
    private const int MaxRetryCount = 3;
    private const int RetryDelay = 2000;
    private readonly string _clientId;
    private readonly string _clientSecret;

    private readonly HttpClient _httpClient;
    private readonly IEveGlobalRateLimit _glodalRateLimit;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly IRedisProvider _redisProvider;
    private readonly IConfiguration _config;
    private readonly ILogger<EveApiClientProvider> _logger;



    public EveApiClientProvider(
        HttpClient httpClient,
        IRedisProvider redisProvider,
        ILogger<EveApiClientProvider> logger,
        IConfiguration config,
        IEveGlobalRateLimit glodalRateLimit)
    {
        _httpClient = httpClient;
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
        _logger = logger;
        _redisProvider = redisProvider;
        _glodalRateLimit = glodalRateLimit;
        _config = config;
        _clientId = _config["ESI:ClientId"];
        _clientSecret = _config["ESI:ClientSecret"];
    }

    public async Task<Result<ICollection<TypeOrdersInfo>>> FetchOrdersForTypeIdAsync(
        int regionId,
        int typeId,
        CancellationToken token,
        string orderType = "all")
    {
        var allData = new ConcurrentBag<TypeOrdersInfo>();

        var page = 1;
        var attempts = 0;

        //HttpResponseMessage response = null;
        var baseKey = $"{GlobalKeysCacheConstants.ETagForOrdersWithType}:{typeId}:{regionId}:";
        var key = $"{baseKey}{page}";

        string baseUrl =
            @$"markets/{regionId}/orders/?datasource=tranquility&order_type={orderType}&type_id={typeId}&page=";
        var url = $"{baseUrl}{page}";
        try
        {
            var countPages = await ProcessFetching(key, url, allData, token);

            if (countPages > 1)
            {
                await LoadMorePageAsync(countPages, baseKey, baseUrl, allData, token);
            }
        }
        catch (HttpRequestException e)
        {
            _logger.LogWarning($"Bad request from url: {url} start retry number {attempts}");
            return Error.InternalServer(e.Message);
        }
        catch (Exception e)
        {
            return Error.InternalServer(e.Message);
        }
        return allData.Any() ? allData.ToList() : Error.NotModified();
    }

    public async Task<Result<ICollection<TypeMarketHistoryInfo>>> FetchMarketHistoryForTypeIdAsync(
        int typeId,
        int regionId,
        CancellationToken token)
    {
        var key = $"{GlobalKeysCacheConstants.ETagForHistoryWithType}:{typeId}:{regionId}";
        string url =
            @$"markets/{regionId}/history/?datasource=tranquility&type_id={typeId}";
        var allData = new ConcurrentBag<TypeMarketHistoryInfo>();

        var attempts = 0;

        while (attempts <= MaxRetryCount)
        {
            try
            {
                await ProcessFetching(key, url, allData, token);
                break; ;
            }
            catch (HttpRequestException e)
            {
                if (++attempts > MaxRetryCount)
                    return Error.InternalServer(e.Message);
                _logger.LogWarning($"Bad request from url: {url} start retry number {attempts}");
                await Task.Delay(RetryDelay, token);
            }
            catch (Exception e)
            {
                return Error.InternalServer(e.Message);
            }
        }
        return allData.Any() ? allData.ToList() : Error.NotModified();
    }

    public async Task<Result<List<TypeOrdersInfo>>> FetchAllOrdersAsync(
        int regionId,
        CancellationToken token)
    {
        var page = 1;
        var countPages = 1;

        string baseUrl =
            @$"markets/{regionId}/orders/?datasource=tranquility&order_type=all&page=";
        var baseKey = $"{GlobalKeysCacheConstants.ETagForAllOrders}:{regionId}:";

        var allData = new ConcurrentBag<TypeOrdersInfo>();

        var url = $"{baseUrl}{page}";
        var key = $"{baseKey}{page}";

        try
        {
            countPages = await ProcessFetching(key, url, allData, token);

            if (countPages > 1)
                await LoadMorePageAsync(countPages, baseKey, baseUrl, allData, token);
        }
        catch (HttpRequestException e)
        {
            _logger.LogWarning($"Bad request start from url {url}");
            return Error.InternalServer(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError($"Exception occurred with message: {e.Message}");
            return Error.InternalServer(e.Message);
        }

        return allData.Any() ? allData.ToList() : Error.NotModified();
    }

    public async Task<Result<JwksMetadata>> FetchJwksMetadataAsync(CancellationToken token)
    {
        try
        {
            var metadataResponse = await _glodalRateLimit.RunTaskAsync(
                async () => await _httpClient.GetAsync(EveConstants.MetadataUrl),
                true,
                token);

            if (!metadataResponse.IsSuccessStatusCode)
                return Error.InternalServer("");

            var metadata = await HandleRequestAsync<MetadataResponse>(metadataResponse, token);
            if (metadata.IsFailure)
                return Error.InternalServer();

            var jwksResponse = await _glodalRateLimit.RunTaskAsync(
                async () => await _httpClient.GetAsync(metadata.Value.JwksUri),
                true,
                token);

            if (!jwksResponse.IsSuccessStatusCode)
                return Error.InternalServer("");

            var jwksMetadata = await HandleRequestAsync<JwksMetadata>(jwksResponse, token);
            if (jwksMetadata.IsFailure)
                return Error.InternalServer();

            return jwksMetadata.Value is not null ? jwksMetadata.Value : Error.InternalServer();
        }
        catch (Exception ex)
        {
            return Error.InternalServer($"Fetichin canceled with error: {ex.Message}");
        }
    }

    public async Task<Result<TokenResponse>> ExchangeCodeForTokenAsync(
        string code,
        CancellationToken token)
    {

        if (string.IsNullOrEmpty(code))
            return Error.InternalServer($"code is null or emty");

        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));

        using var request = new HttpRequestMessage(HttpMethod.Post, EveConstants.LoginUrlToken)
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Basic", credentials) },
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", code }
            })
        };

        try
        {
            var response = await _glodalRateLimit.RunTaskAsync(
               async () => await _httpClient.SendAsync(request),
               false,
               token);

            if (!response.IsSuccessStatusCode)
                return Error.InternalServer("Auth faled");

            var tokenData = await response.Content.ReadFromJsonAsync<TokenResponse>(
                _serializerOptions);

            return tokenData is not null ? tokenData : Error.InternalServer("Auth faled");
        }
        catch (Exception ex)
        {
            return Error.InternalServer(ex.Message);
        }
    }

    private async Task<int> ProcessFetching<T>(
        string key,
        string url,
        ConcurrentBag<T> allData,
        CancellationToken token)
    {
        var response = await _glodalRateLimit.RunTaskAsync(
                async () => await SendRequestWithEtag(url, key, token),
                false,
                token);

        var dataResult = await HandleRequestAsync<List<T>>(
            response,
            key,
            token);

        if (dataResult.IsSuccess)
        {
            var data = dataResult.Value;

            if (data.Count > 0)
            {
                foreach (var item in data)
                {
                    allData.Add(item);
                }
            }
        }

        return GetPageCount(response);
    }

    private async Task<HttpResponseMessage> SendRequestWithEtag(
        string url,
        string key,
        CancellationToken token)
    {
        var eTag = await _redisProvider.GetAsync<string>(key, token);

        _logger.LogInformation($"Start load for url : {url}");
        var request = new HttpRequestMessage(HttpMethod.Get, url);

        if (!string.IsNullOrEmpty(eTag))
            request.Headers.Add("If-None-Match", eTag);

        return await _httpClient.SendAsync(request, token);
    }

    private async Task LoadMorePageAsync<T>(
        int countPages,
        string baseKey,
        string baseUrl,
        ConcurrentBag<T> allData,
        CancellationToken token)
    {
        var tasks = new List<Task>();
        for (int i = 2; i <= countPages; i++)
        {
            var page = i;
            var key = $"{baseKey}{page}";
            string url = @$"{baseUrl}{page}";

            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    await ProcessFetching(key, url, allData, token);
                }
                catch (HttpRequestException e)
                {
                    _logger.LogError($"{e.Message} from url : {url}");
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
                finally
                {
                    _logger.LogInformation($"============ Cancel load for {url}/{countPages} ===================");
                }
            }));
        }
        await Task.WhenAll(tasks);
    }

    private int GetPageCount(HttpResponseMessage response)
    {
        if (response.Headers.TryGetValues("X-Pages", out var pageCount))
            return int.Parse(pageCount.First());
        return 1;
    }

    private async Task<Result<T>> HandleRequestAsync<T>(
        HttpResponseMessage response,
        string key,
        CancellationToken token)
    {
        if (response.StatusCode == HttpStatusCode.NotModified)
        {
            return Error.NotModified();
        }
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"bad request with status code {response.StatusCode}");
        }

        var newEtag = response.Headers.ETag?.Tag;

        if (newEtag is not null)
        {
            await _redisProvider.SetAsync(
                key,
                newEtag,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.UtcNow.AddHours(1)
                },
                token);
        }

        var data = await response.Content.ReadFromJsonAsync<T>(_serializerOptions, token);

        return data is null ? Error.NotModified() : data;
    }
    private async Task<Result<T>> HandleRequestAsync<T>(
        HttpResponseMessage response,
        CancellationToken token)
    {
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"bad request with status code {response.StatusCode}");
        }

        var data = await response.Content.ReadFromJsonAsync<T>(_serializerOptions, token);

        return data is null ? Error.NotModified() : data;
    }
}
