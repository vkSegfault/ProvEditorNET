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

    public async Task CreateAsync(Country country, CancellationToken cancellationToken = default)
    {
        await _context.Countries.AddAsync(country, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Country>> GetAllCountriesAsync(CancellationToken cancellationToken = default)
    {
        var countries = await _context.Countries.ToListAsync(cancellationToken);
        
        return countries;
    }

    public async Task<Country> GetCountryByNameAsync(string countryName, CancellationToken cancellationToken = default)
    {
        var country = await _context.Countries.FirstOrDefaultAsync(x => x.Name == countryName, cancellationToken);
        return country;
    }
    
    public async Task<bool> DeleteCountryAsync(string countryName, CancellationToken cancellationToken = default)
    {
        var country = await _context.Countries.FirstOrDefaultAsync(x => x.Name == countryName, cancellationToken);
        if (country != null)
        {
            _context.Countries.Remove(country);
            Console.WriteLine("Country deleted: " + countryName );
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        
        return false;
    }
    
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}