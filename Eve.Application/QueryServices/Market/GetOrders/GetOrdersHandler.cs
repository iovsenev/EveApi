using AutoMapper;
using Eve.Application.DTOs;
using Eve.Application.QueryServices.Stations.GetStations;
using Eve.Domain.Common;
using Eve.Domain.Constants;
using Eve.Domain.Interfaces.ApiServices;
using Eve.Domain.Interfaces.CacheProviders;
using Eve.Domain.Interfaces.ExternalServices;

namespace Eve.Application.QueryServices.Market.GetOrders;
public class GetOrdersHandler : IRequestHandler<GetOrdersResponse, GetOrdersRequest>
{
    private readonly IRedisProvider _cacheProvider;
    private readonly IEveApiOpenClientProvider _apiClient;
    private readonly IMapper _mapper;
    private readonly IService<StationNameDto> _stationName;

    public GetOrdersHandler(
        IRedisProvider cacheProvider,
        IEveApiOpenClientProvider apiClient,
        IMapper mapper,
        IService<StationNameDto> stationName)
    {
        _cacheProvider = cacheProvider;
        _apiClient = apiClient;
        _mapper = mapper;
        _stationName = stationName;
    }

    public async Task<Result<GetOrdersResponse>> Handle(GetOrdersRequest request, CancellationToken token)
    {
        var key = $"{GlobalKeysCacheConstants.OrdersKey}:{request.RegionId}:{request.TypeId}";

        var result = await _cacheProvider.GetOrSetAsync(
            key,
            () => _apiClient.FetchOrdersForTypeIdAsync(request.RegionId, request.TypeId, token),
            token);

        if (result.IsFailure)
            return result.Error;

        List<TypeOrderDto> orders = new();


        foreach (var orderEntity in result.Value)
        {
            var stationName = await _stationName.Handle(orderEntity.LocationId, token);
            var order = _mapper.Map<TypeOrderDto>(orderEntity);
            order.StationName = stationName.IsSuccess
                ? stationName.Value.Name
                : "unknown";
            orders.Add(order);
        }

        var response = GetResponse(
            orders);

        return response;
    }

    private GetOrdersResponse GetResponse(ICollection<TypeOrderDto> orders)
    {
        var buyOrders = orders
            .Where(o => o.IsBuyOrder);
        var buyJita = buyOrders
            .Where(o => o.SystemId == (int)CentralHubSystemId.Jita)
            .OrderByDescending(o => o.Price);

        var sellOrders = orders
            .Where(o => !o.IsBuyOrder);
        var sellJita = sellOrders
            .Where(o => o.SystemId == (int)CentralHubSystemId.Jita)
            .OrderBy(o => o.Price);


        return new GetOrdersResponse(
            BuyOrders: buyOrders.ToList(),
            SellOrders: sellOrders.ToList());
    }
}
