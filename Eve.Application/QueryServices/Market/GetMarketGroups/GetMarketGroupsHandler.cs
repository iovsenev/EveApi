using AutoMapper;
using Eve.Application.DTOs;
using Eve.Application.QueryServices.CommonRequests;
using Eve.Domain.Common;
using Eve.Domain.Constants;
using Eve.Domain.Interfaces.ApiServices;
using Eve.Domain.Interfaces.CacheProviders;
using Eve.Domain.Interfaces.DataBaseAccess.Read;
using Microsoft.Extensions.Logging;

namespace Eve.Application.QueryServices.Market.GetMarketGroups;
public class GetMarketGroupsHandler : IRequestHandler<GetMarketGroupsResponse, GetCommonEmptyRequest>
{
    private readonly IReadMarketGroupRepository _repos;
    private readonly IRedisProvider _redisCache;
    private readonly IMapper _mapper;
    private readonly ILogger<GetMarketGroupsHandler> _logger;

    public GetMarketGroupsHandler(
        IReadMarketGroupRepository repos,
        IRedisProvider redisCache,
        IMapper mapper,
        ILogger<GetMarketGroupsHandler> logger)
    {
        _repos = repos;
        _redisCache = redisCache;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<GetMarketGroupsResponse>> Handle(GetCommonEmptyRequest req, CancellationToken token)
    {
        var key = $"{GlobalKeysCacheConstants.MarketGroups}:all";

        var result = await _redisCache.GetOrSetAsync(key, () => _repos.GetAllAsync(token), token);

        if (result.IsFailure)
            return result.Error;

        var groups = result.Value
            .Select(g => _mapper.Map<MarketGroupDto>(g))
            .ToList();

        return new GetMarketGroupsResponse(groups);
    }
}
