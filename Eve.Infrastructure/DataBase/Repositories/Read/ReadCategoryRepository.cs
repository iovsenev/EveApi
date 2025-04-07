using Eve.Domain.Common;
using Eve.Domain.Entities;
using Eve.Domain.Interfaces.DataBaseAccess.Read;
using Eve.Infrastructure.DataBase.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Eve.Infrastructure.DataBase.Repositories.Read;
public class ReadCategoryRepository : IReadCategoryRepository
{
    private readonly IAppDbContext _appDbContext;

    public ReadCategoryRepository(IAppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<Result<ICollection<CategoryEntity>>> GetCategoryWithProduct(CancellationToken token)
    {
        var query = _appDbContext.Categories
            .AsNoTracking()
            .Where(c => c.Groups
                .Any(g => g.Types.Any(t => t.IsProduct && t.Published)));

        var categories = await query.ToListAsync();
        if (!categories.Any())
            return Error.NotFound($"No entities found");

        return categories;
    }
}
