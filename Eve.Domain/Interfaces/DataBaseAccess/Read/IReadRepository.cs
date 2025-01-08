using Eve.Domain.Common;

namespace Eve.Domain.Interfaces.DataBaseAccess.Read;
public interface IReadRepository<TEntity>
{
    Task<Result<TEntity>> GetByIdAsync(int id, CancellationToken token);
    Task<Result<ICollection<TEntity>>> GetAllAsync(CancellationToken token);
}
