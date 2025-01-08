using Eve.Domain.Interfaces.ApiServices;

namespace Eve.Application.Services.MarketGroups.GetMarketHistory;

public record GetMarketHistoryRequest(int RegionId, int TypeId) : IRequest;