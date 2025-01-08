using Eve.Domain.Entities;

namespace Eve.Domain.Interfaces.DataBaseAccess.Write;
public interface ICategoryRepository : IWriteRepository<CategoryEntity>
{
    Task AddRangeAsync(List<CategoryEntity> entities, CancellationToken token);
}
