using StackExchange.Redis;

namespace ProvEditorNET.Interfaces;

public interface IRedisService
{
    Task<T> GetAsync<T>(string redisKey);
    Task SetAsync<T>(string redisKey, T value);
    Task<T> GetSetAsync<T>(string redisKey, T value);
    
    // Task<T> TryGetSet<T>(string redisKey, T data);
    Task InvalidateCacheAsync(string redisKey);
}