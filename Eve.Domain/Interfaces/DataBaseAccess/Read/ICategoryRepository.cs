using Eve.Domain.Common;
using Eve.Domain.Entities;

namespace Eve.Domain.Interfaces.DataBaseAccess.Read;
public interface IReadCategoryRepository
{
    Task<Result<ICollection<CategoryEntity>>> GetCategoryWithProduct(CancellationToken token);
    Task<Result<ICollection<GroupEntity>>> GetGroupsForCategoryIdWithProducts(int id, CancellationToken token);
    Task<Result<ICollection<TypeEntity>>> GetTypeIsProductForGroupId(int id, CancellationToken token);
}