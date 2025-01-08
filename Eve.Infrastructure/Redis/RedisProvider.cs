using Eve.Domain.Common;
using Eve.Domain.Interfaces.CacheProviders;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Concurrent;
using System.Text.Json;

namespace Eve.Infrastructure.Redis;
public class RedisProvider : IRedisProvider
{
    private readonly IDistributedCache _cache;
    private static ConcurrentDictionary<string, string> _keys = new();


    public RedisProvider(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken token)
        where T : class
    {
        var result = await _cache.GetStringAsync(key, token);
        if (result == null)
            return null;
        var entity = JsonSerializer.Deserialize<T>(result);

        return entity;
    }

    public async Task SetAsync<T>(string key, T type, CancellationToken token)
        where T : class
    {
        var serializedType = JsonSerializer.Serialize(type);

        await _cache.SetStringAsync(key, serializedType, token);
        _keys.TryAdd(key, key);
    }

    public async Task SetAsync<T>(string key, T type, DistributedCacheEntryOptions options, CancellationToken token)
        where T : class
    {
        var serializedType = JsonSerializer.Serialize(type);

        await _cache.SetStringAsync(key, serializedType, options, token);

        _keys.TryAdd(key, key);
    }

    public async Task RemoveAsync(string key, CancellationToken token)
    {
        await _cache.RemoveAsync(key, token);
        _keys.Remove(key, out _);
    }

    public async Task RemoveByPrefixAsync(string PrefixKey, CancellationToken token)
    {
        var taskList = _keys.Keys
            .Where(k => k.StartsWith(PrefixKey))
            .Select(k => RemoveAsync(k, token));

        await Task.WhenAll(taskList);
    }

    public async Task<Result<TValue>> GetOrSetAsync<TValue>(string key, Func<Task<Result<TValue>>> func, CancellationToken token)
        where TValue :  class
    {
        var entity = await GetAsync<TValue>(key, token);

        if (entity is not null)
            return entity;

        var result = await func();
        if (result.IsFailure)
            return result.Error;
        entity = result.Value;

        await SetAsync(key, entity, token);

        return entity;
    }

    public async Task<Result<T>> GetOrSetAsync<T>(string key, Func<Task<Result<T>>> func, DistributedCacheEntryOptions options, CancellationToken token)
        where T : class
    {
        var entity = await GetAsync<T>(key, token);
        if (entity is not null) return entity;

        var result = await func();

        if (result.IsFailure)
            return result.Error;
        entity = result.Value;

        await SetAsync(key, entity, options, token);

        return entity;
    }
}
