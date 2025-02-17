using Eve.Domain.Common;
using Eve.Domain.Interfaces.DataBaseAccess.Read;
using Eve.Infrastructure.DataBase.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Eve.Infrastructure.DataBase.Repositories.Read;
public class ReadRegionRepository : IReadRegionRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<ReadRegionRepository> _logger;

    public ReadRegionRepository(
        AppDbContext context,
        ILogger<ReadRegionRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<List<int>>> GetAllIdRegions(CancellationToken token)
    {
        try
        {
            var regions = await _context.Regions
                .AsNoTracking()
                .Select(r => r.Id)
                .ToListAsync();

            if (regions is null || !regions.Any())
                return Error.InternalServer("not found any regions");

            return regions;


        }
        catch (Exception ex)
        {
            _logger.LogError($"Load data for regions canceled with error message : {ex.Message}");
            return Error.InternalServer($"Data load error");
        }

    }
}
