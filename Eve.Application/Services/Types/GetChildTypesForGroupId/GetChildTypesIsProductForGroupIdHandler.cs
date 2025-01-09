using AutoMapper;
using Eve.Application.DTOs;
using Eve.Domain.Common;
using Eve.Domain.Constants;
using Eve.Domain.Interfaces.ApiServices;
using Eve.Domain.Interfaces.CacheProviders;
using Eve.Domain.Interfaces.DataBaseAccess.Read;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Eve.Application.Services.Types.GetChildTypesForGroupId;

public class GetChildTypesIsProductForGroupIdHandler : IRequestHandler<GetChildTypesProductResponse, GetChildTypesRequest>
{
    private readonly IReadCategoryRepository _repos;
    private readonly IMapper _mapper;
    private readonly IRedisProvider _redisCache;
    private readonly ILogger<GetChildTypesForMarketGroupIdHandler> _logger;

    public GetChildTypesIsProductForGroupIdHandler(
        IReadCategoryRepository repos,
        IMapper mapper,
        IRedisProvider redisCache,
        ILogger<GetChildTypesForMarketGroupIdHandler> logger)
    {
        _repos = repos;
        _mapper = mapper;
        _redisCache = redisCache;
        _logger = logger;
    }

    public async Task<Result<GetChildTypesProductResponse>> Handle(GetChildTypesRequest request, CancellationToken token)
    {
        var groupId = request.GroupId;
        var key = $"{GlobalKeysCacheConstants.MarketGroupTypes}:{groupId}";

        var result = await _redisCache.GetOrSetAsync(
            key,
            () => _repos.GetTypeIsProductForGroupId(groupId, token),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddDays(1)
            },
            token);

        if (result.IsFailure) return result.Error;

        var types = result.Value
            .Select(t => _mapper.Map<ShortTypeDto>(t))
            .ToList();

        return new GetChildTypesProductResponse(types);
    }
}
