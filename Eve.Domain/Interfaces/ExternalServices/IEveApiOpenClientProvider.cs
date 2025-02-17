using Eve.Domain.Common;
using Eve.Domain.ExternalTypes;

namespace Eve.Domain.Interfaces.ExternalServices;
public interface IEveApiOpenClientProvider
{
    Task<Result<ICollection<TypeOrdersInfo>>> FetchOrdersForTypeIdAsync(int regionId, int typeId, CancellationToken token, string orderType = "all");
    Task<Result<ICollection<TypeMarketHistoryInfo>>> FetchMarketHistoryForTypeIdAsync(int typeId, int regionId, CancellationToken token);
    Task<Result<List<TypeOrdersInfo>>> FetchAllOrdersAsync(int regionId, CancellationToken token);
}