using Microsoft.EntityFrameworkCore;
using ProvEditorNET.Interfaces;
using ProvEditorNET.Models;
using ProvEditorNET.Repository;

namespace ProvEditorNET.Services;

public class ProvinceService : IProvinceService
{
    private readonly ProvinceDbContext _context;

    public ProvinceService(ProvinceDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(Province province)
    {
        await _context.Provinces.AddAsync(province);
        await _context.SaveChangesAsync();
    }
    
    public async Task<IEnumerable<Province>> GetAllProvincesAsync()
    {
        var provinces = await _context.Provinces.Include(p => p.Country).ToListAsync();
        return provinces;
    }

    public async Task<Province> GetProvinceByNameAsync(string provinceName)
    {
        // .Include() does eager loading of Country relation as a part of Province query to DB
        // https://learn.microsoft.com/en-us/ef/core/querying/related-data/
        var province = await _context.Provinces.Include(p => p.Country).FirstOrDefaultAsync(x => x.Name == provinceName);
        return province;
    }
    
    public async Task<bool> DeleteProvinceAsync(string provinceName)
    {
        var province = await _context.Provinces.FirstOrDefaultAsync(x => x.Name == provinceName);
        if (province != null)
        {
            _context.Provinces.Remove(province);
            Console.WriteLine("Province deleted: " + provinceName );
            await _context.SaveChangesAsync();
            return true;
        }
        
        return false;
    }
}