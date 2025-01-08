using AutoMapper;
using Eve.Application.DTOs;
using Eve.Domain.Common;
using Eve.Domain.Constants;
using Eve.Domain.Interfaces.ApiServices;
using Eve.Domain.Interfaces.CacheProviders;
using Eve.Domain.Interfaces.DataBaseAccess.Read;
using Eve.Domain.Interfaces.ExternalServices;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Eve.Application.Services.Types.GetTypeForId;
public class GetTypeForIdHandler : IRequestHandler<GetTypeForIdResponse, GetTypeForIdRequest>
{
    private readonly ITypeReadRepository _repository;
    private readonly IRedisProvider _cacheProvider;
    private readonly IEveApiClientProvider _apiProvider;
    private readonly IMapper _mapper;
    private readonly ILogger<GetTypeForIdHandler> _logger;
    public GetTypeForIdHandler(
        ITypeReadRepository repository,
        IRedisProvider cacheProvider,
        IEveApiClientProvider apiProvider,
        IMapper mapper,
        ILogger<GetTypeForIdHandler> logger)
    {
        _repository = repository;
        _cacheProvider = cacheProvider;
        _apiProvider = apiProvider;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<GetTypeForIdResponse>> Handle(GetTypeForIdRequest request, CancellationToken token)
    {
        var typeId = request.TypeId;
        var key = $"{GlobalKeysCacheConstants.Type}:{typeId}";
        var type = await _cacheProvider.GetAsync<TypeInfoDto>(key, token);

        if (type is null)
        {
            var result = await ReadTypeFromDatabase(typeId, token);

            if (result.IsFailure)
                return result.Error;

            type = result.Value;

            await _cacheProvider.SetAsync(
                key,
                type, new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(1)
                },
                token
            );
        }

        double summPriceMaterials = 0;

        foreach (var item in type.ReprocessComponents)
        {
            var price = await GetBestPrice(item.TypeId, token);

            if (price.IsFailure)
                return price.Error;

            summPriceMaterials += price.Value;
        }

        return new GetTypeForIdResponse(type, summPriceMaterials);
    }

    private async Task<Result<TypeInfoDto>> ReadTypeFromDatabase(int typeId, CancellationToken token)
    {
        var typeResult = await _repository.GetByIdAsync(typeId, token);

        if (typeResult.IsFailure)
            return typeResult.Error;

        var materialsResult = await _repository.GetReprocessMaterialsForTypeId(typeId, token);

        if (materialsResult.IsFailure)
            return materialsResult.Error;

        var type = _mapper.Map<TypeInfoDto>(typeResult.Value);

        var materials = materialsResult.Value
            .Select(m => _mapper.Map<MaterialDto>(m))
            .ToList();

        type.ReprocessComponents = materials;

        return type;
    }

    private async Task<Result<double>> GetBestPrice(int typeId, CancellationToken token)
    {
        var key = $"{GlobalKeysCacheConstants.OrdersKey}:{(int)CentralHubRegionId.Jita}:{typeId}";

        var orders = await _cacheProvider.GetOrSetAsync(
            key,
            () => _apiProvider.GetTypeOrdersInfo(
                (int)CentralHubRegionId.Jita,
                typeId, token),
            new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddHours(4)
                },
            token);

        if (orders.IsFailure)
            return orders.Error;

        var bestSellOrder = orders.Value
            .Where(o=> !o.IsBuyOrder)
            .OrderByDescending(o => o.Price)
            .Select(o => o.Price)
            .Take(1)
            .FirstOrDefault();

        return bestSellOrder == default
            ? Error.InternalServer($"Not found orders for id: {typeId}") 
            : bestSellOrder;
    }
}
