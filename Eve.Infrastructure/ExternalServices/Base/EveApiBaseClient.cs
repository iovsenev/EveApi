using Eve.Domain.Common;
using Eve.Infrastructure.ExternalServices.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Eve.Infrastructure.ExternalServices.Base;

public abstract class EveApiBaseClient
{
    protected readonly HttpClient _httpClient;
    protected readonly IEveGlobalRateLimit _rateLimit;
    protected readonly IEveRetryPolicyProvider _retryPolicy;
    protected readonly ILogger _logger;
    protected readonly IConfiguration _configuration;

    protected readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        WriteIndented = false
    };

    protected readonly int MaxDegreeOfParallelism;

    protected EveApiBaseClient(
        HttpClient httpClient,
        IEveGlobalRateLimit globalRateLimit,
        IEveRetryPolicyProvider retryPolicy,
        ILogger logger,
        IConfiguration config
        )
    {
        _httpClient = httpClient;
        _rateLimit = globalRateLimit;
        _retryPolicy = retryPolicy;
        _logger = logger;
        _configuration = config;

        MaxDegreeOfParallelism = _configuration.GetValue<int>("ESI:MaxDegreeOfParallelism");
    }

    #region CoreMethods
    protected async Task<Result<T>> SendRequestAsync<T>(
        HttpRequestMessage request,
        CancellationToken token,
        bool isServiceRequest = true,
        string? eTag = null)
    {
        if (eTag is not null)
        {
            request.Headers.Add("If-None-Match", eTag);
        }
        try
        {
            var policy = _retryPolicy.GetEntityRetryPolicy<T>();
            return await policy.ExecuteAsync(async () =>
            {
                _logger.LogInformation($"Start fetching for URL: {request.RequestUri}.");
                var response = await _rateLimit.RunTaskAsync(
                    async () => await _httpClient.SendAsync(request, token),
                    isServiceRequest,
                    token);


                return await HandleResponseAsync<T>(response, token);
            });
        }
        catch (HttpRequestException reqEx)
        {
            _logger.LogError(reqEx, "HTTP error for {Url}", request.RequestUri);
            return Error.InternalServer($"HTTP error: {reqEx.StatusCode}");
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning("Request to {Url} was canceled", request.RequestUri);
            return Error.Cancelled();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error for {Url}", request.RequestUri);
            return Error.InternalServer(ex.Message);
        }
    }

    protected async Task<Result<T>> HandleResponseAsync<T>(
        HttpResponseMessage response,
        CancellationToken token)
    {
        switch (response.StatusCode)
        {
            case HttpStatusCode.NotModified:
                return Error.NotModified();
            case HttpStatusCode.TooManyRequests:
                _logger.LogWarning("Rate limit exceeded for {Url}", response.RequestMessage?.RequestUri);
                return Error.TooManyRequests();
            case HttpStatusCode.Unauthorized:
                return Error.Unauthorized();
            case >= HttpStatusCode.InternalServerError:
                _logger.LogError("Server error {StatusCode} for {Url}",
                    response.StatusCode, response.RequestMessage?.RequestUri);
                return Error.InternalServer($"Server error: {response.StatusCode}");
        }

        try
        {
            var content = await response.Content.ReadAsStreamAsync(token);

            var data = await JsonSerializer.DeserializeAsync<T>(content, SerializerOptions, token);

            return data is not null
                ? data
                : Error.InternalServer("Null response Data");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Json deserialize error");
            return Error.InternalServer("Invalid JSON format");
        }
    }
    #endregion

    #region Pagination
    protected async Task<Result<List<T>>> FetchPaginatedDataAsync<T>(
        string baseUrl,
        CancellationToken token,
        bool isServiceRequest = true,
        int maxPages = int.MaxValue,
        Action<int, int> progressCallback = null)
    {
        var results = new List<T>();
        var currentPage = 1;
        var totalPages = 1;
        var url = $"{baseUrl}{(baseUrl.Contains('?') ? '&' : '?')}page=1";
        totalPages = await GetTotalPages(url, token);


        while (currentPage <= totalPages && currentPage <= maxPages)
        {
            progressCallback?.Invoke(currentPage, totalPages);

            url = $"{baseUrl}{(baseUrl.Contains('?') ? '&' : '?')}page={currentPage}";
            var result = await SendRequestAsync<List<T>>(
                new HttpRequestMessage(HttpMethod.Get, url),
                token,
                isServiceRequest);

            if (result.IsFailure)
            {
                return currentPage == 1 ? result : results;
            }

            results.AddRange(result.Value);
            currentPage++;
        }
        return results;
    }


    protected async Task<Result<List<T>>> FetchPaginatedDataParallelAsync<T>(
        string baseUrl,
        CancellationToken token,
        bool isServiceRequest = true,
        int maxDegreeOfParallelism = 10)
    {
        var totalPages = await GetTotalPages(baseUrl, token);

        if (totalPages == 1)
        {
            return await FetchPaginatedDataAsync<T>(baseUrl, token, isServiceRequest, totalPages);
        }

        var results = new ConcurrentBag<T>();
        var pages = Enumerable.Range(1, totalPages);

        await Parallel.ForEachAsync(
            pages,
            new ParallelOptions
            {
                MaxDegreeOfParallelism = maxDegreeOfParallelism,
                CancellationToken = token
            },
            async (page, ct) =>
            {
                var url = $"{baseUrl}{(baseUrl.Contains('?') ? '&' : '?')}page={page}";
                var result = await SendRequestAsync<List<T>>(
                    new HttpRequestMessage(HttpMethod.Get, url),
                    ct,
                    isServiceRequest);

                if (result.IsSuccess)
                {
                    foreach (var item in result.Value)
                        results.Add(item);
                }
            });
        return results.ToList();
    }

    private async Task<int> GetTotalPages(
        string url,
        CancellationToken token)
    {
        var request = new HttpRequestMessage(HttpMethod.Head, url);

        try
        {
            var response = await _rateLimit.RunTaskAsync(
                        async () => await _httpClient.SendAsync(request, token),
                        true,
                        token);

            return int.Parse(response.Headers.GetValues("X-Pages").FirstOrDefault() ?? "1");
        }
        catch
        {
            return 1;
        }

    }
    #endregion

    #region Helpers
    protected HttpRequestMessage CreateAuthRequest(
        HttpMethod method,
        string url,
        Dictionary<string, string> formData,
        string clientId,
        string clientSecret)
    {
        var request = new HttpRequestMessage(method, url)
        {
            Content = new FormUrlEncodedContent(formData)
        };

        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Basic",
            Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"))
        );

        return request;
    }

    protected HttpRequestMessage CreateJsonRequest<T>(
        HttpMethod method,
        string url,
        T data)
    {
        return new HttpRequestMessage(method, url)
        {
            Content = JsonContent.Create(data, options: SerializerOptions)
        };
    }
    #endregion

}



