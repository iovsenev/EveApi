using Eve.Domain.Common;
using Eve.Domain.Constants;
using Eve.Domain.ExternalTypes;
using Eve.Domain.Interfaces.CacheProviders;
using Eve.Domain.Interfaces.DataBaseAccess.Read;
using Eve.Domain.Interfaces.ExternalServices;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Eve.Application.InternalServices;
public class LoadOrdersService : ILoadOrdersService
{
    private readonly ILogger<LoadOrdersService> _logger;
    private readonly IReadRegionRepository _regionRepository;
    private readonly IEveApiMarketProvider _httpClient;
    private readonly IRedisProvider _redisProvider;

    public LoadOrdersService(
        ILogger<LoadOrdersService> logger,
        IReadRegionRepository regionRepository,
        IEveApiMarketProvider httpClient,
        IRedisProvider redisProvider)
    {
        _logger = logger;
        _regionRepository = regionRepository;
        _httpClient = httpClient;
        _redisProvider = redisProvider;
    }

    public async Task<bool> RunTaskAsync(CancellationToken token)
    {
        var notLoadRegionsId = new List<int>();
        try
        {
            var result = await _regionRepository.GetAllIdRegionsIDs(token);
            if (result.IsFailure)
                throw new Exception(result.Error.Message);

            if (!result.Value.Any())
                throw new Exception("not any regions ids");

            var regionsIds = result.Value
                .OrderBy(x => x)
                .ToList();

            notLoadRegionsId = await Load(regionsIds, token);

            if (notLoadRegionsId.Count == 0)
                return true;

            _logger.LogWarning("Not all regions load");

            for (int i = 0; i < 5; i++)
            {
                await Task.Delay(60000);

                _logger.LogInformation($"Retry for {i + 1} part start at {DateTime.Now}");
                notLoadRegionsId = await Load(notLoadRegionsId, token);

                if (notLoadRegionsId.Count == 0)
                    return true;
            }

            throw new Exception("not all regions load");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Load data canceled with error : {ex.Message}", ex);
            return false;
        }
    }

    private async Task<List<int>> Load(List<int> regionsIds, CancellationToken token)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();

        var notLoadRegionsId = new List<int>();
        var position = 0;

        while (regionsIds.Count > position)
        {
            var regionId = regionsIds[position];
            _logger.LogInformation($"Starting Load orders from ESI at {DateTime.UtcNow} for id : {regionId} position : {position}");

            var response = await RequestForOrders(regionId, token);

            if (response.IsFailure)
            {
                if (response.Error.ErrorCode == ErrorCodes.NotModified)
                {
                    position++;
                    continue;
                }
                _logger.LogWarning($"Not Loaded orders for region id : {regionId}");
                notLoadRegionsId.Add(regionId);
            }
            var result = await LoadCacheOrders(response.Value, regionId, token);

            if (result.IsFailure)
            {
                _logger.LogWarning($"Not loaded orders in to cache for region id {regionId}");
                notLoadRegionsId.Add(regionId);
            }
            position++;
        }

        stopWatch.Stop();
        var milSec = stopWatch.ElapsedMilliseconds / 1000;
        _logger.LogInformation($"Loading all regions continue at {milSec} sec");
        return notLoadRegionsId;
    }

    private async Task<Result<Dictionary<int, List<TypeOrdersInfo>>>> RequestForOrders(int regionId, CancellationToken token)
    {
        try
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var result = await _httpClient.GetOrdersForRegionAsync(regionId, token);

            if (result.IsFailure)
                return result.Error.ErrorCode == ErrorCodes.NotModified 
                    ? result.Error 
                    : throw new Exception(result.Error.Message);

            stopWatch.Stop();
            var milSec = stopWatch.ElapsedMilliseconds;
            _logger.LogInformation($"fetching data completed in {milSec} for region id {regionId}");

            var orders = result.Value
                .GroupBy(o => o.TypeId)
                .ToDictionary(x => x.Key, 
                              x => x.Select(o => o)
                                    .ToList());

            return orders;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error to fetching from httpClient: {ex.Message}", ex);
            return Error.InternalServer(ex.Message);
        }

    }

    private async Task<Result> LoadCacheOrders(Dictionary<int, List<TypeOrdersInfo>> orders, int regionId, CancellationToken token)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        try
        {
            foreach (var order in orders)
            {
                var typeId = order.Key;
                var key = $"{GlobalKeysCacheConstants.OrdersKey}:{typeId}:{regionId}";
                await _redisProvider.SetAsync(
                    key,
                    order.Value,
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.UtcNow.AddHours(3)
                    }, token);
            }

            stopWatch.Stop();
            var milSec = stopWatch.ElapsedMilliseconds;
            _logger.LogInformation($"writing data in redis completed in {milSec} for region id : {regionId}");

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error to redis adding with message: {ex.Message}", ex);
            return Error.InternalServer(ex.Message);
        }
    }
}
