using AutoMapper.Configuration.Conventions;
using Eve.Domain.Interfaces.ApiServices;

namespace Eve.Application.QueryServices.Market.GetOrders;

public record GetOrdersRequest(int TypeId, int RegionId) : IRequest;