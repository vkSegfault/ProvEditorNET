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
    
            // var countries = await _cache.GetAsync(
        //     "countries",   // retrieve `countrties` from cache (cache hit)
        //     async cancellationToken =>   // call database directly if nothing is in cache (token is set to cancel this if something is in cache) -> cache miss
        //     {
        //         var countries = await _context.Countries.ToListAsync(_cancellationToken);
        //         return countries;
        //     }
        //     );
        
        // var cachedCountries = await _cache.GetAsync("dupa", cancellationToken); 
        
        // if( cancellationToken != null )
        // {
        //     Console.WriteLine("Countries retrieved from cache");
        //     Console.WriteLine($"Cached Countries: {cachedCountries}");
        //     
        // }

        // await _cache.GetStringAsync( "dupa", cancellationToken );
        
        // string key = "countries";
        // byte[] countriesBytes = await _cache.GetAsync(key);
        // if( countriesBytes is not null )
        // {
        //     Console.WriteLine("Countries found in Redis cache ");
        //     List<Country> countries = JsonSerializer.Deserialize<List<Country>>(countriesBytes);
        //     return countries;
        // }
        // else
        // {
        //     Console.WriteLine("No countries in Redis cache - using DB and creating new cache entry");
        //     IEnumerable<Country> countries = await _context.Countries.ToListAsync();
        //
        //     // var bytes = JsonSerializer.SerializeToUtf8Bytes(countries);
        //     // JsonSerializer.Serialize<List<Country>>(countries);  // modelBuilder.Entity<Order>() .Property(e => e.Price) .HasConversion( v => JsonSerializer.Serialize(v), v => JsonSerializer.Deserialize<Money>(v)); 
        //     // byte[] countriesBytes2 = Encoding.UTF8.GetBytes( JsonSerializer.Serialize( countries ) );
        //     // await _cache.SetAsync(key, countriesBytes2);
        //     
        //     return countries;
        // }
        
        // byte[] valueBytes = Encoding.UTF8.GetBytes("mamuta");
        // // byte[] valueBytesCountries = 
        // await _cache.SetAsync(key, valueBytes);
        //
        // // await _cache.RemoveAsync(key);
        //
        // // both below are equivalent
        // string value = await _cache.GetStringAsync(key);
        // byte[] valueBytes2 = await _cache.GetAsync(key);
        // string value2 = null;
        // if (valueBytes2 != null)
        // {
        //     value2 = Encoding.UTF8.GetString(valueBytes2);
        // }
        //
        // if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(value2))
        // {
        //     Console.WriteLine( "No cache entry found" );
        // }
        // else
        // {
        //     Console.WriteLine( $"Redis key '{key}' has value '{value}' and value '{value2}'" );
        // }
}