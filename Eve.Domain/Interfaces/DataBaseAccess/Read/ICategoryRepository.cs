using Eve.Domain.Common;
using Eve.Domain.Entities;

namespace Eve.Domain.Interfaces.DataBaseAccess.Read;
public interface IReadCategoryRepository
{
    Task<Result<ICollection<CategoryEntity>>> GetCategoryWithProduct(CancellationToken token);
}