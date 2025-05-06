namespace Eve.Infrastructure.ExternalServices.Interfaces;

public interface IEveGlobalRateLimit
{
    Task<HttpResponseMessage> RunTaskAsync( Func<Task<HttpResponseMessage>> taskToRun, bool isServiceRequest, CancellationToken token);
}