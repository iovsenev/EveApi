using AutoMapper;
using Eve.Application.DTOs;
using Eve.Application.QueryServices.CommonRequests;
using Eve.Domain.Common;
using Eve.Domain.Constants;
using Eve.Domain.Interfaces.ApiServices;
using Eve.Domain.Interfaces.CacheProviders;
using Eve.Domain.Interfaces.DataBaseAccess.Read;
using Eve.Domain.Interfaces.ExternalServices;
using Microsoft.Extensions.Caching.Distributed;

namespace Eve.Application.QueryServices.Types.GetTypeForId;
public class GetTypeForIdHandler : IRequestHandler<GetTypeForIdResponse, GetCommonRequestForId>
{
    private readonly IReadTypeRepository _repository;
    private readonly IRedisProvider _cacheProvider;
    private readonly IEveApiOpenClientProvider _apiProvider;
    private readonly IMapper _mapper;
    public GetTypeForIdHandler(
        IReadTypeRepository repository,
        IRedisProvider cacheProvider,
        IEveApiOpenClientProvider apiProvider,
        IMapper mapper)
    {
        _repository = repository;
        _cacheProvider = cacheProvider;
        _apiProvider = apiProvider;
        _mapper = mapper;
    }

    public async Task<Result<GetTypeForIdResponse>> Handle(GetCommonRequestForId request, CancellationToken token)
    {
        var typeId = request.Id;
        var key = $"{GlobalKeysCacheConstants.Type}:{typeId}";

        var result = await _cacheProvider.GetOrSetAsync(
            key,
            () => ReadTypeFromDatabase(typeId, token),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(1)
            },
            token);

        if (result.IsFailure)
            return result.Error;

        double summPriceMaterials = 0;

        foreach (var item in result.Value.ReprocessComponents)
        {
            var price = await GetBestPrice(item.TypeId, token);

            if (price.IsFailure)
                return price.Error;

            summPriceMaterials += price.Value;
        }

        return new GetTypeForIdResponse(result.Value, summPriceMaterials);
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
            () => _apiProvider.FetchOrdersForTypeIdAsync(
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
            .Where(o => !o.IsBuyOrder)
            .OrderByDescending(o => o.Price)
            .Select(o => o.Price)
            .Take(1)
            .FirstOrDefault();

        return  bestSellOrder;
    }
}
