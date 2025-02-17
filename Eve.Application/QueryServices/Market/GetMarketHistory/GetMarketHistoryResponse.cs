using Eve.Domain.ExternalTypes;
using Eve.Domain.Interfaces.ApiServices;

namespace Eve.Application.QueryServices.Market.GetMarketHistory;

public record GetMarketHistoryResponse(ICollection<TypeMarketHistoryInfo> History);