using Eve.Domain.Common;
using Eve.Domain.Entities;

namespace Eve.Domain.Interfaces.DataBaseAccess.Read;
public interface IReadMarketGroupRepository : IReadRepository<MarketGroupEntity>
{
    Task<Result<ICollection<TypeEntity>>> GetChildTypesAsync(int id, CancellationToken token);
}
