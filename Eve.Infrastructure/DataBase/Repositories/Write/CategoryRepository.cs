using Eve.Domain.Common;
using Eve.Domain.Entities;
using Eve.Domain.Interfaces.DataBaseAccess.Write;
using Eve.Infrastructure.DataBase.Contexts;

namespace Eve.Infrastructure.DataBase.Repositories.Write;
public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddRangeAsync(List<CategoryEntity> entities, CancellationToken token)
    {
        await _context.Categories.AddRangeAsync(entities, token);

        await SaveChangesAsync(token);
    }

    public Task<int> CreateAsync(CategoryEntity entity, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(int id, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task<ICollection<CategoryEntity>> GetAllAsync(CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task<Result<CategoryEntity>> GetByIdAsync(int id, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task<int> UpdateAsync(CategoryEntity entity, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task UpdateRangeAsync(IEnumerable<CategoryEntity> entity, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    private async Task<int> SaveChangesAsync(CancellationToken token)
    {
        return await _context.SaveChangesAsync(token);
    }
}
