using AutoMapper.Configuration.Conventions;
using Eve.Domain.Interfaces.ApiServices;

namespace Eve.Application.Services.MarketGroups.GetOrders;

public record GetOrdersRequest(int TypeId, int RegionId) : IRequest;