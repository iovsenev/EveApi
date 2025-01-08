using AutoMapper;
using Eve.Application.DTOs;
using Eve.Domain.Common;
using Eve.Domain.Constants;
using Eve.Domain.ExternalTypes;
using Eve.Domain.Interfaces.ApiServices;
using Eve.Domain.Interfaces.CacheProviders;
using Eve.Domain.Interfaces.DataBaseAccess.Read;
using Eve.Domain.Interfaces.ExternalServices;
using Microsoft.Extensions.Caching.Distributed;

namespace Eve.Application.Services.Products;
public class GetProductHandler : IRequestHandler<GetProductResponse, GetProductRequest>
{
    private readonly IProductRepository _repository;
    private readonly IRedisProvider _cacheProvider;
    private readonly IMapper _mapper;
    private readonly IEveApiClientProvider _apiClient;

    public GetProductHandler(
        IProductRepository repository,
        IRedisProvider cacheProvider,
        IMapper mapper,
        IEveApiClientProvider apiClient)
    {
        _repository = repository;
        _cacheProvider = cacheProvider;
        _mapper = mapper;
        _apiClient = apiClient;
    }

    public async Task<Result<GetProductResponse>> Handle(GetProductRequest request, CancellationToken token)
    {
        var key = $"{GlobalKeysCacheConstants.Product}:{request.TypeId}";

        var blueprintCoeffEff = (100 - request.BlueprintEff) / 100;
        var structCoeffEff = (100 - request.StructEff) / 100;

        var result = await _cacheProvider.GetAsync<ProductDto>(key, token);

        if (result is null)
        {
            var productDataBase = await ReadInDatabase(request.TypeId, token);

            if (productDataBase.IsFailure)
                return productDataBase.Error;
            result = productDataBase.Value;

            await _cacheProvider.SetAsync(
                key,
                result,
                new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromSeconds(10)
                },
                token);
        }

        var averagePriceType = await GetPriceForType(request.TypeId, token);

        if (averagePriceType.IsFailure)
            return averagePriceType.Error;

        double sumPriceMaterialsSell = 0;
        double sumPriceMaterialsBuy = 0;

        foreach (var item in result.Materials)
        {
            var price = await GetPriceForType(item.TypeId, token);
            if (price.IsFailure)
                return price.Error;

            item.Quantity = (int)Math.Ceiling(item.Quantity * blueprintCoeffEff * structCoeffEff);
            sumPriceMaterialsBuy += price.Value.buy * item.Quantity;
            sumPriceMaterialsSell += price.Value.sell * item.Quantity;
        }

        sumPriceMaterialsBuy = Math.Round(sumPriceMaterialsBuy, 2);
        sumPriceMaterialsSell = Math.Round(sumPriceMaterialsSell, 2);

        return new GetProductResponse(
            result,
            BuyPrice: averagePriceType.Value.buy,
            SellPrice: averagePriceType.Value.sell,
            BuyPriceMaterials: sumPriceMaterialsBuy,
            SellPriceMaterials: sumPriceMaterialsSell
            );
    }

    private async Task<Result<ProductDto>> ReadInDatabase(int typeId, CancellationToken token)
    {
        ProductDto result;

        var productEntity = await _repository.GetProductForId(typeId, token);

        if (productEntity.IsFailure)
            return productEntity.Error;

        result = _mapper.Map<ProductDto>(productEntity.Value);

        var materialsEntity = await _repository.GetMaterialsForProductId(result.Id, token);

        if (materialsEntity.IsFailure)
            return materialsEntity.Error;

        var materials = materialsEntity.Value
            .Select(m => _mapper.Map<MaterialDto>(m))
            .ToList();

        result.Materials = materials;
        return result;
    }

    private async Task<Result<(double buy, double sell)>> GetPriceForType(int typeId, CancellationToken token)
    {
        var key = $"{GlobalKeysCacheConstants.OrdersKey}:{(int)CentralHubRegionId.Jita}:{typeId}";

        var result = await _cacheProvider.GetAsync<ICollection<TypeOrdersInfo>>(key, token);
        if (result == null)
        {
            var resultQuery = await _apiClient.GetTypeOrdersInfo((int)CentralHubRegionId.Jita, typeId, token);
            if (resultQuery.IsFailure) return resultQuery.Error;
            result = resultQuery.Value;

            await _cacheProvider.SetAsync(key, result, new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddHours(4)
            },
                token);
        }
        double buyAverage = 0;
        double sellAverage = 0;

        var buy = result
            .Where(o => o.IsBuyOrder);
        if (buy.Any())
        {
            buyAverage = buy
                 .OrderByDescending(o => o.Price)
                 .Select(o => o.Price)
                 .Take(3)
                 .Average();
        }

        var sell = result
            .Where(o => !o.IsBuyOrder);

        if (sell.Any())
        {
            sellAverage = sell
                .OrderBy(o => o.Price)
                 .Select(o => o.Price)
                 .Take(3)
                 .Average();
        }

        buyAverage = Math.Round(buyAverage, 2, MidpointRounding.AwayFromZero);
        sellAverage = Math.Round(sellAverage, 2, MidpointRounding.AwayFromZero);

        return (buyAverage, sellAverage);
    }
}
