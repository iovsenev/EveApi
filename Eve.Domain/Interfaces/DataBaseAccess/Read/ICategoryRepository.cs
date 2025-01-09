using Eve.Domain.Common;
using Eve.Domain.Entities;

namespace Eve.Domain.Interfaces.DataBaseAccess.Read;
public interface IReadCategoryRepository
{
    Task<ICollection<CategoryEntity>> GetCategoryForPruduct(CancellationToken token);
    Task<ICollection<GroupEntity>> GetGroupsForCategoryIdWithProducts(int id, CancellationToken token);
    Task<Result<ICollection<TypeEntity>>> GetTypeIsProductForGroupId(int id, CancellationToken token);
}