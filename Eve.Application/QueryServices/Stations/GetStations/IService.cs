using Eve.Application.DTOs;
using Eve.Domain.Common;

namespace Eve.Application.QueryServices.Stations.GetStations;
public interface IService<TValue> where TValue : class
{
    Task<Result<TValue>> Handle(long id, CancellationToken token);
}