using Eve.Domain.Common;
using Eve.Domain.Constants;
using Eve.Domain.ExternalTypes;

namespace Eve.Domain.Interfaces.ExternalServices;
public interface IEveApiMarketProvider
{
    Task<Result<List<TypeMarketHistoryInfo>>> GetMarketHistoryAsync(int regionId, int typeId, CancellationToken token);
    Task<Result<List<TypeOrdersInfo>>> GetOrdersForRegionAsync(
        int regionId, 
        CancellationToken token, 
        int? typeId = null, 
        OrderType orderType = OrderType.All, 
        int maxPages = 100);
}