using AutoMapper;
using Eve.Application.DTOs;
using Eve.Application.QueryServices.CommonRequests;
using Eve.Domain.Common;
using Eve.Domain.Constants;
using Eve.Domain.Interfaces.ApiServices;
using Eve.Domain.Interfaces.CacheProviders;
using Eve.Domain.Interfaces.DataBaseAccess.Read;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Eve.Application.QueryServices.Types.GetChildTypesForMarketGroup;
public class GetChildTypesForMarketGroupIdHandler : IRequestHandler<GetChildTypesResponse, GetCommonRequestForId>
{
    private readonly IMarketReadRepository _repos;
    private readonly IMapper _mapper;
    private readonly IRedisProvider _redisCache;
    private readonly ILogger<GetChildTypesForMarketGroupIdHandler> _logger;

    public GetChildTypesForMarketGroupIdHandler(
        IMarketReadRepository repos,
        IMapper mapper,
        IRedisProvider redisCache,
        ILogger<GetChildTypesForMarketGroupIdHandler> logger)
    {
        _repos = repos;
        _mapper = mapper;
        _redisCache = redisCache;
        _logger = logger;
    }

    public async Task<Result<GetChildTypesResponse>> Handle(GetCommonRequestForId request, CancellationToken token)
    {
        var groupId = request.Id;

        var result = await _repos.GetChildTypesAsync(groupId, token);

        if (result.IsFailure) return result.Error;

        var types = result.Value
            .Select(t => _mapper.Map<ShortTypeDto>(t))
            .ToList();

        return new GetChildTypesResponse(types);
    }
}
