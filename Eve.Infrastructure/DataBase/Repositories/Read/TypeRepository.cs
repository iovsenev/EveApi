using Eve.Domain.Common;
using Eve.Domain.Entities;
using Eve.Domain.Interfaces.DataBaseAccess.Read;
using Eve.Infrastructure.DataBase.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Eve.Infrastructure.DataBase.Repositories.Read;
public class TypeRepository : ITypeReadRepository
{
    private readonly AppDbContext _context;

    public TypeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ICollection<TypeEntity>>> GetTypesByNameConteins(string containedName, CancellationToken token)
    {
        var types = await _context.Types
            .Where(t => t.Published && t.MarketGroup != null && t.Name
                                                .ToLower()
                                                .StartsWith(containedName
                                                            .ToLower()))
            .Include(t => t.Icon)
            .OrderBy(t => t.Name)
            .ToListAsync(token);

        return types;
    }

    public Task<int> CreateAsync(TypeEntity entity, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(int id, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<TypeEntity>> GetByIdAsync(int id, CancellationToken token)
    {
        var type = await _context.Types
            .AsNoTracking()
            .Include(t => t.Icon)
            .FirstOrDefaultAsync(t => t.Id == id, token);

        return type == null ? Error.NotFound($"The type is not found for id: {id}") : type;
    }

    public async Task<Result<ICollection<ReprocessMaterialEntity>>>GetReprocessMaterialsForTypeId(int typeId, CancellationToken token)
    {
        var materials = await _context.ReprocessMaterials
            .AsNoTracking()
            .Where(m => m.TypeId == typeId)
            .Include(m => m.Material)
            .ToListAsync(token);

        return materials;
    }

    public Task<Result<ICollection<TypeEntity>>> GetAllAsync(CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task<int> UpdateAsync(TypeEntity entity, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task UpdateRangeAsync(IEnumerable<TypeEntity> entity, CancellationToken token)
    {
        throw new NotImplementedException();
    }

}
