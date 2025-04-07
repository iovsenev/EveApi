using Eve.Domain.Common;
using Eve.Domain.Entities;
using Eve.Domain.Interfaces.DataBaseAccess.Read;
using Eve.Infrastructure.DataBase.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Eve.Infrastructure.DataBase.Repositories.Read;
public class ReadMarketGroupRepository : IReadMarketGroupRepository
{
    private readonly IAppDbContext _context;

    public ReadMarketGroupRepository(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ICollection<TypeEntity>>> GetChildTypesAsync(int id, CancellationToken token)
    {
        var types = await _context.Types
            .AsNoTracking()
            .Where(t => t.MarketGroupId == id && t.Published == true)
            .Include(g => g.Icon)
            .ToListAsync();

        if (!types.Any()) return Error.NotFound($"The child types not found for group id: ");

        return types;
    }

    public async Task<Result<MarketGroupEntity>> GetByIdAsync(int id, CancellationToken token)
    {
        var marketGroup = await _context.MarketGroups
            .AsNoTracking()
            .Include(g => g.Icon)
            .FirstOrDefaultAsync(g => g.Id == id);

        return marketGroup is not null 
            ? marketGroup 
            : Error.NotFound($"Group with id: {id} is not found");

    }

    public async Task<Result<ICollection<MarketGroupEntity>>> GetAllAsync(CancellationToken token)
    {
        var entities = await _context.MarketGroups
            .AsNoTracking()
            .Include(g => g.Icon)
            .ToListAsync();

        return entities.Any() ? entities : Error.NotFound("no entities not found");
    }
}
