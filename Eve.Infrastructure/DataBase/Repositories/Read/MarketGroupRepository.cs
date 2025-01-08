using Eve.Domain.Common;
using Eve.Domain.Entities;
using Eve.Domain.Interfaces.DataBaseAccess.Read;
using Eve.Infrastructure.DataBase.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Eve.Infrastructure.DataBase.Repositories.Read;
public class MarketGroupRepository : IMarketReadRepository
{
    private readonly AppDbContext _context;

    public MarketGroupRepository(AppDbContext context)
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
        Console.WriteLine(types.Count);
        if (!types.Any()) return Error.NotFound($"The child types not found for group id: ");

        return types;
    }

    public async Task<Result<MarketGroupEntity>> GetByIdAsync(int id, CancellationToken token)
    {
        var type = await _context.MarketGroups
            .AsNoTracking()
            .Include(g => g.Icon)
            .FirstOrDefaultAsync(g => g.Id == id);

        return type is not null 
            ? type 
            : Error.NotFound($"Group with id: {id} is not found");

    }

    public async Task<Result<ICollection<MarketGroupEntity>>> GetAllAsync(CancellationToken token)
    {
        var entities = await _context.MarketGroups
            .AsNoTracking()
            .Include(g => g.Icon)
            .ToListAsync();

        return entities;
    }
}
