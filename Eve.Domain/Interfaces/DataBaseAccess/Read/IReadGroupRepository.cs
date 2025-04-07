using Eve.Domain.Common;
using Eve.Domain.Entities;

namespace Eve.Domain.Interfaces.DataBaseAccess.Read;

public interface IReadGroupRepository
{
    Task<Result<ICollection<GroupEntity>>> GetGroupsForCategoryIdWithProducts(int id, CancellationToken token);
}