using Eve.Domain.Common;
using Eve.Domain.Interfaces.DataBaseAccess.Read;
using Eve.Infrastructure.DataBase.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Eve.Infrastructure.DataBase.Repositories.Read;
public class ReadRegionRepository : IReadRegionRepository
{
    private readonly IAppDbContext _context;

    public ReadRegionRepository(
        IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<int>>> GetAllIdRegions(CancellationToken token)
    {
            var regions = await _context.Regions
                .AsNoTracking()
                .Select(r => r.Id)
                .ToListAsync();

            if (regions is null || !regions.Any())
                return Error.InternalServer("not found any regions");

            return regions;
    }
}
