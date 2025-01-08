using Eve.Domain.Common;

namespace Eve.Domain.Interfaces.DataBaseAccess.Write;

public interface IWriteRepository<TEntity>
{
    Task<int> CreateAsync(TEntity entity, CancellationToken token);
    Task DeleteAsync(int id, CancellationToken token);
    Task<Result<TEntity>> GetByIdAsync(int id, CancellationToken token);
    Task<ICollection<TEntity>> GetAllAsync(CancellationToken token);
    Task<int> UpdateAsync(TEntity entity, CancellationToken token);
    Task UpdateRangeAsync(IEnumerable<TEntity> entity, CancellationToken token);
}
