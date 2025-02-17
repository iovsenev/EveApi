
namespace Eve.Application.InternalServices;

public interface ILoadOrdersService
{
    Task<bool> RunTaskAsync(CancellationToken stoppingToken);
}