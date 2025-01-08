using Eve.Application.DTOs;
using Eve.Domain.Interfaces.ApiServices;

namespace Eve.Application.Services.MarketGroups.GetOrders;

public record GetOrdersResponse(
    ICollection<TypeOrderDto> BuyOrders, 
    ICollection<TypeOrderDto> SellOrders);