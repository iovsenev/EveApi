using Eve.Application.DTOs;
using Eve.Domain.Interfaces.ApiServices;

namespace Eve.Application.QueryServices.Market.GetOrders;

public record GetOrdersResponse(
    ICollection<TypeOrderDto> BuyOrders,
    ICollection<TypeOrderDto> SellOrders);