using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ProvEditorNET.Interfaces;
using ProvEditorNET.Models;
using ProvEditorNET.Repository;

namespace ProvEditorNET.Services;

public class CountryService : ICountryService
{
    private readonly ProvinceDbContext _context;

    public CountryService(ProvinceDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(Country country)
    {
        await _context.Countries.AddAsync(country);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Country>> GetAllCountriesAsync()
    {
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
        
        var countries = await _context.Countries.ToListAsync();
        
        return countries;
    }

    public async Task<Country> GetCountryByNameAsync(string countryName)
    {
        var country = await _context.Countries.FirstOrDefaultAsync(x => x.Name == countryName);
        return country;
    }
    
    public async Task<bool> DeleteCountryAsync(string countryName)
    {
        var country = await _context.Countries.FirstOrDefaultAsync(x => x.Name == countryName);
        if (country != null)
        {
            _context.Countries.Remove(country);
            Console.WriteLine("Country deleted: " + countryName );
            await _context.SaveChangesAsync();
            return true;
        }
        
        return false;
    }
}