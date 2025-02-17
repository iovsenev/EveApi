using Eve.Domain.Common;
using Eve.Domain.Entities;
using Eve.Domain.Interfaces.DataBaseAccess.Read;
using Eve.Infrastructure.DataBase.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Eve.Infrastructure.DataBase.Repositories.Read;
public class ReadCategoryRepository : IReadCategoryRepository
{
    private readonly AppDbContext _appDbContext;

    public ReadCategoryRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<Result<ICollection<CategoryEntity>>> GetCategoryForPruduct(CancellationToken token)
    {
        var categories = await _appDbContext.Categories
            .AsNoTracking()
            .Where(c => c.Groups
                .Any(g => g.Types.Any(t => t.IsProduct && t.Published)))
            .ToListAsync();

        if (!categories.Any())
            return Error.NotFound($"No entities found");

        return categories;
    }

    public async Task<Result<ICollection<GroupEntity>>> GetGroupsForCategoryIdWithProducts(int id, CancellationToken token)
    {
        var groups = await _appDbContext.Groups
            .AsNoTracking()
            .Where(g => g.CategoryId == id && g.Types.Any(t => t.IsProduct && t.Published))
            .ToListAsync();

        return groups;
    }

    public async Task<Result<ICollection<TypeEntity>>> GetTypeIsProductForGroupId(int id, CancellationToken token)
    {
        var types = await _appDbContext.Types
            .AsNoTracking()
            .Where(t => t.IsProduct && t.Published && t.GroupId == id)
            .Include(t => t.Icon)
            .ToListAsync();

        if (!types.Any())
            return Error.NotFound($"No entity found for group id {id}");

        return types;
    }
}
