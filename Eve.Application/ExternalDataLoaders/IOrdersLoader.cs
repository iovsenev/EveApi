using Eve.Domain.Common;

namespace Eve.Application.ExternalDataLoaders;
public interface IOrdersLoader
{
    Task<Result> Load(int regionId, CancellationToken token);
}