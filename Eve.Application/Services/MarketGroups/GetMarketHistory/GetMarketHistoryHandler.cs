using AutoMapper;
using Eve.Domain.Common;
using Eve.Domain.Constants;
using Eve.Domain.ExternalTypes;
using Eve.Domain.Interfaces.ApiServices;
using Eve.Domain.Interfaces.CacheProviders;
using Eve.Domain.Interfaces.ExternalServices;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Eve.Application.Services.MarketGroups.GetMarketHistory;
public class GetMarketHistoryHandler : IRequestHandler<GetMarketHistoryResponse, GetMarketHistoryRequest>
{
    private readonly IRedisProvider _cacheProvider;
    private readonly IEveApiClientProvider _apiClientProvider;
    private readonly ILogger<GetMarketHistoryHandler> _logger;

    public GetMarketHistoryHandler(
        IRedisProvider cacheProvider,
        IEveApiClientProvider apiClientProvider,
        ILogger<GetMarketHistoryHandler> logger)
    {
        _cacheProvider = cacheProvider;
        _apiClientProvider = apiClientProvider;
        _logger = logger;
    }

    public async Task<Result<GetMarketHistoryResponse>> Handle(GetMarketHistoryRequest request, CancellationToken token)
    {
        var key = $"{GlobalKeysCacheConstants.OrdersHistoryKey}:{request.RegionId}:{request.TypeId}";

        var result = await _cacheProvider.GetOrSetAsync(
            key,
            () => _apiClientProvider.GetTypeHistoryInfo(
                typeId: request.TypeId,
                regionId: request.RegionId,
                token),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Today.AddDays(1)
            },
            token);

        if (result.IsFailure) 
            return result.Error;

        var history = CreateResponse(result.Value);

        return new GetMarketHistoryResponse(history);
    }

    private ICollection<TypeMarketHistoryInfo> CreateResponse(ICollection<TypeMarketHistoryInfo> history)
    {
        return history
            .OrderByDescending(h => h.Date)
            .ToList();
    }
}
