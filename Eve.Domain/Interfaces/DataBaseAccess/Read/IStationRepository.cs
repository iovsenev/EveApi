using Eve.Domain.Common;
using Eve.Domain.Entities;

namespace Eve.Domain.Interfaces.DataBaseAccess.Read;
public interface IStationRepository
{
    Task<Result<StationEntity>> GetStationById(long id, CancellationToken token);
}