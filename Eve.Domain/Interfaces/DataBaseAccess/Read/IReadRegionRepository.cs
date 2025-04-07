using Eve.Domain.Common;

namespace Eve.Domain.Interfaces.DataBaseAccess.Read;
public interface IReadRegionRepository
{
    Task<Result<List<int>>> GetAllIdRegionsIDs(CancellationToken token);
}