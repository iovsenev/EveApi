using Eve.Domain.Common;
using Eve.Domain.Entities;

namespace Eve.Domain.Interfaces.DataBaseAccess.Read;
public interface IReadTypeReadRepository 
{
    Task<Result<ICollection<ReprocessMaterialEntity>>> GetReprocessMaterialsForTypeId(int typeId, CancellationToken token);
    Task<Result<ICollection<TypeEntity>>> GetTypesByNameContains(string containedName, CancellationToken token);
    Task<Result<TypeEntity>> GetByIdAsync(int id, CancellationToken token);
    Task<Result<ICollection<TypeEntity>>> GetTypeIsProductForGroupId(int id, CancellationToken token);
}