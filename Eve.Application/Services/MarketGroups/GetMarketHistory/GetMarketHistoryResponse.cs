using Eve.Domain.ExternalTypes;
using Eve.Domain.Interfaces.ApiServices;

namespace Eve.Application.Services.MarketGroups.GetMarketHistory;

public record GetMarketHistoryResponse(ICollection<TypeMarketHistoryInfo> History);