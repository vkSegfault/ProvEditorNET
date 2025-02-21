using System.Text;
using System.Text.Json;
using ProvEditorNET.Interfaces;

namespace ProvEditorNET.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;


public class RedisService : IRedisService
{
    private readonly IDistributedCache _cache;

    public RedisService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T> GetAsync<T>(string redisKey)
    {
        byte[] bytes = await _cache.GetAsync(redisKey);
        if (bytes == null)
        {
            Console.WriteLine($"{redisKey} not found in cache");
            T value = default;
            return value!;
        }
        else
        {
            Console.WriteLine($"{redisKey} found in cache");
            // Encoding.UTF8.GetString(bytes);
            return JsonSerializer.Deserialize<T>(bytes);
        }
    }

    public async Task SetAsync<T>(string redisKey, T value)
    {
        Console.WriteLine($"{redisKey} set to cache");
        var bytes = JsonSerializer.SerializeToUtf8Bytes(value);
        
        var options = new DistributedCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(30))
            .SetAbsoluteExpiration(TimeSpan.FromHours(1));
        
        await _cache.SetAsync(redisKey, bytes, options);
    }

    public async Task<T> GetSetAsync<T>(string redisKey, T value)
    {
        var cached = await GetAsync<T>(redisKey);

        if (cached != null)
        {
            return cached;         
        }
        else
        {
            await SetAsync<T>(redisKey, value);
            return cached!;
        }
    }
    
    public async Task InvalidateCacheAsync(string redisKey)
    {
        Console.WriteLine($"{redisKey} removed from cache");
        await _cache.RemoveAsync(redisKey);   // invalidate cache every time we change data
    }
}