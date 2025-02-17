
namespace Eve.Infrastructure.ExternalServices;

public interface IEveGlobalRateLimit
{
    Task<HttpResponseMessage> RunTaskAsync( Func<Task<HttpResponseMessage>> taskToRun, bool isServiceRequest, CancellationToken token);
}