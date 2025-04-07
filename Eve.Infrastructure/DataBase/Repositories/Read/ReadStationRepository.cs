using Eve.Domain.Common;
using Eve.Domain.Entities;
using Eve.Domain.Interfaces.DataBaseAccess.Read;
using Eve.Infrastructure.DataBase.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Eve.Infrastructure.DataBase.Repositories.Read;
public class ReadStationRepository : IReadStationRepository
{
    private readonly IAppDbContext _context;

    public ReadStationRepository(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<StationEntity>> GetStationById(long id, CancellationToken token)
    {
        var station = await _context.Stations
            .AsNoTracking()
            .FirstOrDefaultAsync(station => station.Id == id);

        if (station is null)
            return Error.NotFound($"Not found station with id: {id}");

        return station;
    }
}
