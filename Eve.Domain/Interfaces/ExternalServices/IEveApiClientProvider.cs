using Eve.Domain.Common;
using Eve.Domain.ExternalTypes;

namespace Eve.Domain.Interfaces.ExternalServices;
public interface IEveApiClientProvider
{
    Task<Result<ICollection<TypeOrdersInfo>>> GetTypeOrdersInfo(int regionId, int typeId, CancellationToken token, string orderType = "all");
    Task<Result<ICollection<TypeMarketHistoryInfo>>> GetTypeHistoryInfo(int typeId, int regionId, CancellationToken token);
    Task<Result<List<TypeOrdersInfo>>> LoadAllOrdersAsync(int regionId, CancellationToken token);
}