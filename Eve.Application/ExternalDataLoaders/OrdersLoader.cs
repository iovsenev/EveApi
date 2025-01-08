using Eve.Domain.Common;
using Eve.Domain.Constants;
using Eve.Domain.Interfaces.CacheProviders;
using Eve.Domain.Interfaces.ExternalServices;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Eve.Application.ExternalDataLoaders;
public class OrdersLoader : IOrdersLoader
{
    private readonly ILogger<OrdersLoader> _logger;
    private readonly IEveApiClientProvider _httpClient;
    private readonly IRedisProvider _redisProvider;

    public OrdersLoader(
        ILogger<OrdersLoader> logger,
        IEveApiClientProvider httpClient,
        IRedisProvider redisProvider)
    {
        _logger = logger;
        _httpClient = httpClient;
        _redisProvider = redisProvider;
    }

    public async Task<Result> Load(int regionId, CancellationToken token)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();

        var result = await _httpClient.LoadAllOrdersAsync(regionId, token);

        if (result.IsFailure)
            return result.Error;

        stopWatch.Stop();
        var milSec = stopWatch.ElapsedMilliseconds;
        _logger.LogInformation($"fetching data completed in {milSec}");

        var orders = result.Value
            .GroupBy(o => o.TypeId)
            .ToDictionary(x => x.Key, x => x.Select(o => o));

        stopWatch.Restart();

        try
        {
            foreach (var order in orders)
            {
                var key = $"{GlobalKeysCacheConstants.OrdersKey}:{regionId}:{order.Key}";
                await _redisProvider.SetAsync(key, order.Value, token);
            }

            stopWatch.Stop();
            milSec = stopWatch.ElapsedMilliseconds;
            _logger.LogInformation($"writing data in redis completed in {milSec}");

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error to redis adding with message: {ex.Message}", ex);
            return Error.InternalServer(ex.Message);
        }
    }
}