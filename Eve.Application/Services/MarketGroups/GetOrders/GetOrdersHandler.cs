using AutoMapper;
using Eve.Application.DTOs;
using Eve.Application.Services.Stations.GetStations;
using Eve.Domain.Common;
using Eve.Domain.Constants;
using Eve.Domain.ExternalTypes;
using Eve.Domain.Interfaces.ApiServices;
using Eve.Domain.Interfaces.CacheProviders;
using Eve.Domain.Interfaces.ExternalServices;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;

namespace Eve.Application.Services.MarketGroups.GetOrders;
public class GetOrdersHandler : IRequestHandler<GetOrdersResponse, GetOrdersRequest>
{
    private readonly IRedisProvider _cacheProvider;
    private readonly IEveApiClientProvider _apiClient;
    private readonly IMapper _mapper;
    private readonly IService<StationNameDto> _stationName;
    private readonly ILogger<GetOrdersHandler> _logger;

    public GetOrdersHandler(
        IRedisProvider cacheProvider,
        IEveApiClientProvider apiClient,
        IMapper mapper,
        ILogger<GetOrdersHandler> logger,
        IService<StationNameDto> stationName)
    {
        _cacheProvider = cacheProvider;
        _apiClient = apiClient;
        _mapper = mapper;
        _logger = logger;
        _stationName = stationName;
    }

    public async Task<Result<GetOrdersResponse>> Handle(GetOrdersRequest request, CancellationToken token)
    {
        var key = $"{GlobalKeysCacheConstants.OrdersKey}:{request.RegionId}:{request.TypeId}";

        var result = await _cacheProvider.GetOrSetAsync(
            key,
            () => _apiClient.GetTypeOrdersInfo(request.RegionId, request.TypeId, token),
            token);

        if (result.IsFailure)
            return result.Error;

        ConcurrentBag<TypeOrderDto> orders = new();


        foreach(var orderEntity in result.Value)
        {
            var stationName = await _stationName.Handle(orderEntity.LocationId, token);
            var order = _mapper.Map<TypeOrderDto>(orderEntity);
            order.StationName = stationName.IsSuccess
                ? stationName.Value.Name 
                : "unknown";
            orders.Add(order);
        }

        var response = GetResponse(
            orders.ToList());

        return response;
    }

    private GetOrdersResponse GetResponse(ICollection<TypeOrderDto> orders)
    {
        var buyOrders = orders
            .Where(o => o.IsBuyOrder);
        var buyJita = buyOrders
            .Where(o => o.SystemId == (int)CentralHubSystemId.Jita);

        buyOrders = buyJita.Any()
            ? buyJita
                .OrderByDescending(o => o.Price)
                .Take(3)
            : buyOrders
                .OrderByDescending(o => o.Price)
                .Take(3);

        var sellOrders = orders
            .Where(o => !o.IsBuyOrder);
        var sellJita = sellOrders
            .Where(o => o.SystemId == (int)CentralHubSystemId.Jita);

        sellOrders = buyJita.Any()
            ? sellJita
                .OrderByDescending(o => o.Price)
                .Take(3)
            : sellOrders
                .OrderByDescending(o => o.Price)
                .Take(3);

        return new GetOrdersResponse(
            BuyOrders: buyOrders.ToList(),
            SellOrders: sellOrders.ToList());
    }
}
