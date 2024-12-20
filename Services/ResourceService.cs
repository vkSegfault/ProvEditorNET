using ProvEditorNET.Interfaces;
using ProvEditorNET.Models;
using ProvEditorNET.Repository;

namespace ProvEditorNET.Services;

public class ResourceService : IResourceService
{
    private readonly ProvinceDbContext _context;

    public ResourceService(ProvinceDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(Resource resource)
    {
        await _context.Res.AddAsync(resource);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Country>> GetAllCountriesAsync()
    {
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