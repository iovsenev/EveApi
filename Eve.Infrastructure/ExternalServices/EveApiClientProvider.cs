using Eve.Domain.Common;
using Eve.Domain.Constants;
using Eve.Domain.ExternalTypes;
using Eve.Domain.Interfaces.CacheProviders;
using Eve.Domain.Interfaces.ExternalServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Eve.Infrastructure.ExternalServices;

public class EveApiClientProvider : IEveApiClientProvider
{
    private const int MaxConcurrentRequests = 30;
    int _complitedRequest = 0;
    SemaphoreSlim _semaphore = new SemaphoreSlim(MaxConcurrentRequests);
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly IRedisProvider _redisProvider;
    private readonly ILogger<EveApiClientProvider> _logger;

    public EveApiClientProvider(
        HttpClient httpClient,
        IRedisProvider redisProvider,
        ILogger<EveApiClientProvider> logger)
    {
        _httpClient = httpClient;
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
        _logger = logger;
        _redisProvider = redisProvider;
    }

    public async Task<Result<ICollection<TypeOrdersInfo>>> GetTypeOrdersInfo(
        int regionId,
        int typeId,
        CancellationToken token,
        string orderType = "all")
    {
        var page = 1;
        var allData = new List<TypeOrdersInfo>();
        while (true)
        {
            string url =
                @$"https://esi.evetech.net/latest/markets/{regionId}/orders/?datasource=tranquility&order_type={orderType}&page={page}&type_id={typeId}";
            try
            {

                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadFromJsonAsync<ICollection<TypeOrdersInfo>>(_serializerOptions, token);
                if (data == null || data.Count == 0) break;

                allData.AddRange(data);
                page++;
            }
            catch (HttpRequestException e)
            {
                break;
            }
            catch (Exception e)
            {
                return Error.InternalServer(e.Message);
            }
        }
        return allData;
    }

    public async Task<Result<ICollection<TypeMarketHistoryInfo>>> GetTypeHistoryInfo(
        int typeId,
        int regionId,
        CancellationToken token)
    {
        var allData = new List<TypeMarketHistoryInfo>();
        string url =
            @$"https://esi.evetech.net/latest/markets/{regionId}/history/?datasource=tranquility&type_id={typeId}";
        try
        {

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadFromJsonAsync<ICollection<TypeMarketHistoryInfo>>(_serializerOptions, token);
            if (data == null || data.Count == 0)
                return allData;
            allData.AddRange(data);

        }
        catch (HttpRequestException e)
        {
            return Error.BadRequest($"Request to third party api failed with message: {e.Message}");
        }
        catch (Exception e)
        {
            return Error.InternalServer(e.Message);
        }

        return allData;
    }

    public async Task<Result<List<TypeOrdersInfo>>> LoadAllOrdersAsync(
        int regionId,
        CancellationToken token)
    {
        var key = $"{GlobalKeysCacheConstants.OrdersKey}:{GlobalKeysCacheConstants.ETag}";
        var etag = await _redisProvider.GetAsync<string>(key, token);

        var page = 1;

        string baseUrl =
            @$"https://esi.evetech.net/latest/markets/{regionId}/orders/?datasource=tranquility&order_type=all&page=";
        var allData = new ConcurrentBag<TypeOrdersInfo>();

        var response = await SendRequestWithEtag(
            url: $"{baseUrl}{1}",
            token: token,
            eTag: etag);

        if (response.StatusCode == System.Net.HttpStatusCode.NotModified)
        {
            return Error.NotModified();
        }

        if (!response.IsSuccessStatusCode)
        {
            Error.InternalServer();
        }

        var newEtag = response.Headers.ETag?.Tag;

        if (newEtag is not null)
        {
            await _redisProvider.SetAsync(key, newEtag, token);
        }

        var content = await response.Content.ReadFromJsonAsync<ICollection<TypeOrdersInfo>>(_serializerOptions, token);
        if (content is null)
            return Error.NotModified();
        foreach (var item in content)
            allData.Add(item);

        var pageCount = GetPageCount(response);
        if (pageCount <= 1)
            return allData.ToList();

        await FetchPagesWithRateLimitAsync(baseUrl, pageCount, allData, token);

        return allData.ToList();
    }

    private Task<HttpResponseMessage> SendRequestWithEtag(string url, CancellationToken token, string? eTag = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);

        if (!string.IsNullOrEmpty(eTag))
            request.Headers.Add("If-None-Match", eTag);

        return _httpClient.SendAsync(request);
    }

    int GetPageCount(HttpResponseMessage response)
    {
        if (response.Headers.TryGetValues("X-Pages", out var pageCount))
            return int.Parse(pageCount.First());
        return 1;
    }

    private async Task FetchPagesWithRateLimitAsync(
        string baseUrl,
        int totalPages,
        ConcurrentBag<TypeOrdersInfo> allData,
        CancellationToken token)
    {

        var tasks = new List<Task>();
        var stopwatch = Stopwatch.StartNew();
        int page = 2;
        while (page <= totalPages)
        {
            await _semaphore.WaitAsync(token);

            if (page > totalPages)
                break;

            Interlocked.Increment(ref _complitedRequest);
            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    if (page > totalPages)
                        return;
                    await FetchPageAsync($"{baseUrl}{page}", allData, token);
                }
                finally
                {
                    _semaphore.Release();
                }
            }, token));

            if (_complitedRequest >= MaxConcurrentRequests)
            {
                stopwatch.Stop();

                var elapsed = stopwatch.ElapsedMilliseconds;
                if (elapsed < 1000)
                {
                    await Task.Delay(1000 - (int)elapsed, token);
                }
                _complitedRequest = 0;
                stopwatch.Restart();
                _logger.LogInformation($"Loaging {page} pages in {elapsed}ms");
            }

            page++;
        }

        await Task.WhenAll(tasks);

        _logger.LogInformation($"Loading {page - 1} pages");

    }

    private async Task FetchPageAsync(
        string url,
        ConcurrentBag<TypeOrdersInfo> allData,
        CancellationToken token)
    {
        try
        {
            var response = await _httpClient.GetAsync(url, token);

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Error for request : {response.StatusCode}, URL: {url}");

            var content = await response.Content.ReadFromJsonAsync<ICollection<TypeOrdersInfo>>(_serializerOptions, token);

            if (content is not null)
            {
                foreach (var item in content)
                    allData.Add(item);
            }

        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Error for request: {url}. Exceptions: {ex.Message}");
        }


    }
}