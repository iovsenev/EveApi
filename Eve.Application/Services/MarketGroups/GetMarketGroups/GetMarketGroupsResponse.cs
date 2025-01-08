
using Eve.Application.DTOs;
using Eve.Domain.Interfaces.ApiServices;

namespace Eve.Application.Services.MarketGroups.GetMarketGroups;
public record GetMarketGroupsResponse(ICollection<MarketGroupDto> Groups);
