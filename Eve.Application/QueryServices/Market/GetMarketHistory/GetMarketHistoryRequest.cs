using Eve.Domain.Interfaces.ApiServices;

namespace Eve.Application.QueryServices.Market.GetMarketHistory;

public record GetMarketHistoryRequest(int RegionId, int TypeId) : IRequest;