using Eve.Domain.Common;
using Polly.Retry;

namespace Eve.Infrastructure.ExternalServices.Interfaces;
public interface IEveRetryPolicyProvider
{
    AsyncRetryPolicy<Result<T>> GetEntityRetryPolicy<T>();
    AsyncRetryPolicy<HttpResponseMessage> GetHttpRetryPolicy();
    AsyncRetryPolicy<Result<List<T>>> GetPaginatedRetryPolicy<T>();
}
