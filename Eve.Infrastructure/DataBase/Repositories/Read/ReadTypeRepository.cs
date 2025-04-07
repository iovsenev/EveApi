using Eve.Domain.Common;
using Eve.Domain.Entities;
using Eve.Domain.Interfaces.DataBaseAccess.Read;
using Eve.Infrastructure.DataBase.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Eve.Infrastructure.DataBase.Repositories.Read;
public class ReadTypeRepository : IReadTypeReadRepository
{
    private readonly IAppDbContext _context;

    public ReadTypeRepository(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ICollection<TypeEntity>>> GetTypesByNameContains(
        string containedName, 
        CancellationToken token)
    {
        var types = await _context.Types
            .Where(t => t.Published && t.MarketGroupId != null && t.Name
                                                .ToLower()
                                                .StartsWith(containedName
                                                            .ToLower()))
            .Include(t => t.Icon)
            .OrderBy(t => t.Name)
            .ToListAsync(token);

        return types;
    }

    public async Task<Result<TypeEntity>> GetByIdAsync(int TypeId, CancellationToken token)
    {
        var type = await _context.Types
            .AsNoTracking()
            .Include(t => t.Icon)
            .FirstOrDefaultAsync(t => t.Id == TypeId, token);

        return type == null ? Error.NotFound($"The type is not found for id: {TypeId}") : type;
    }

    public async Task<Result<ICollection<ReprocessMaterialEntity>>>GetReprocessMaterialsForTypeId(
        int typeId, 
        CancellationToken token)
    {
        var materials = await _context.ReprocessMaterials
            .AsNoTracking()
            .Where(m => m.TypeId == typeId)
            .Include(m => m.Material)
            .ToListAsync(token);

        return materials;
    }

    public async Task<Result<ICollection<TypeEntity>>> GetTypeIsProductForGroupId(int id, CancellationToken token)
    {
        var types = await _context.Types
            .AsNoTracking()
            .Where(t => t.IsProduct && t.Published && t.GroupId == id)
            .Include(t => t.Icon)
            .ToListAsync(token);

        if (!types.Any())
            return Error.NotFound($"No entity found for group id {id}");

        return types;
    }
}
