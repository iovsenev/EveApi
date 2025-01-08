using Eve.Domain.Common;
using Microsoft.Extensions.Caching.Distributed;

namespace Eve.Domain.Interfaces.CacheProviders;

public interface IRedisProvider
{
    Task<T?> GetAsync<T>(string key, CancellationToken token) where T : class;
    Task<Result<T>> GetOrSetAsync<T>(
        string key,
        Func<Task<Result<T>>> func,
        CancellationToken token) where T : class;
    Task<Result<T>> GetOrSetAsync<T>(
        string key,
        Func<Task<Result<T>>> func,
        DistributedCacheEntryOptions options,
        CancellationToken token) where T : class;
    Task RemoveAsync(string key, CancellationToken token);
    Task RemoveByPrefixAsync(string PrefixKey, CancellationToken token);
    Task SetAsync<T>(string key, T type, CancellationToken token) where T : class;
    Task SetAsync<T>(
        string key,
        T type,
        DistributedCacheEntryOptions options,
        CancellationToken token) where T : class;
}