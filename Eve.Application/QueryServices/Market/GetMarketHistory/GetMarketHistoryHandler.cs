using Eve.Domain.Common;
using Eve.Domain.Constants;
using Eve.Domain.ExternalTypes;
using Eve.Domain.Interfaces.ApiServices;
using Eve.Domain.Interfaces.CacheProviders;
using Eve.Domain.Interfaces.ExternalServices;
using Microsoft.Extensions.Caching.Distributed;

namespace Eve.Application.QueryServices.Market.GetMarketHistory;
public class GetMarketHistoryHandler : IRequestHandler<GetMarketHistoryResponse, GetMarketHistoryRequest>
{
    private readonly IRedisProvider _cacheProvider;
    private readonly IEveApiMarketProvider _apiClientProvider;

    public GetMarketHistoryHandler(
        IRedisProvider cacheProvider,
        IEveApiMarketProvider apiClientProvider)
    {
        _cacheProvider = cacheProvider;
        _apiClientProvider = apiClientProvider;
    }

    public async Task<Result<GetMarketHistoryResponse>> Handle(GetMarketHistoryRequest request, CancellationToken token)
    {
        var key = $"{GlobalKeysCacheConstants.OrdersHistoryKey}:{request.RegionId}:{request.TypeId}";

        var result = await _cacheProvider.GetOrSetAsync(
            key,
            () => _apiClientProvider.GetMarketHistoryAsync(
                regionId: request.RegionId,
                typeId: request.TypeId,
                token),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(1)
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
