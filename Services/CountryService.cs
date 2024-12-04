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
        var countries = await _context.Countries.ToListAsync();
        return countries;
    }

    public async Task<Country> GetCountryByName(string countryName)
    {
        var country = await _context.Countries.FirstOrDefaultAsync(x => x.Name == countryName);
        return country;
    }
}