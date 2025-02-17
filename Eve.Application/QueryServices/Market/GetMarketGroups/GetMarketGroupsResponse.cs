using Eve.Application.DTOs;
using Eve.Domain.Interfaces.ApiServices;

namespace Eve.Application.QueryServices.Market.GetMarketGroups;
public record GetMarketGroupsResponse(ICollection<MarketGroupDto> Groups);
