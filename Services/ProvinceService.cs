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
}