using Eve.Domain.Common;
using Eve.Domain.Entities;

namespace Eve.Domain.Interfaces.DataBaseAccess.Read;
public interface ITypeReadRepository : IReadRepository<TypeEntity>
{
    Task<Result<ICollection<ReprocessMaterialEntity>>> GetReprocessMaterialsForTypeId(int typeId, CancellationToken token);
    Task<Result<ICollection<TypeEntity>>> GetTypesByNameConteins(string containedName, CancellationToken token);
}