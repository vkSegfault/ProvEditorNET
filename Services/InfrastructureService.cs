using Microsoft.EntityFrameworkCore;
using ProvEditorNET.Interfaces;
using ProvEditorNET.Models;
using ProvEditorNET.Repository;

namespace ProvEditorNET.Services;

public class InfrastructureService : IInfrastructureService
{
    private readonly ProvinceDbContext _context;

    public InfrastructureService(ProvinceDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(Infrastructure infrastructure)
    {
        await _context.Infrastructures.AddAsync(infrastructure);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Infrastructure>> GetAllInfrastrcturesAsync()
    {
        var infrastructures = await _context.Infrastructures.ToListAsync();
        return infrastructures;
    }

    public async Task<Infrastructure> GetInfrastructureByNameAsync(string infrastructureName)
    {
        var infrastructure = await _context.Infrastructures.FirstOrDefaultAsync(x => x.Name == infrastructureName);
        return infrastructure;
    }

    public async Task<ICollection<Infrastructure>> GetInfrastructuresFromStringListAsync(IEnumerable<string> infrastructureNames)
    {
        ICollection<Infrastructure> infrastructures = new List<Infrastructure>();
        foreach (var infraName in infrastructureNames)
        {
            Infrastructure infrastructure = await GetInfrastructureByNameAsync(infraName);
            infrastructures.Add( infrastructure );
        }
        
        return infrastructures;
    }
    
    public async Task<bool> DeleteAsync(string infrastructureName)
    {
        var infrastructure = await _context.Infrastructures.FirstOrDefaultAsync(x => x.Name == infrastructureName);
        if (infrastructure != null)
        {
            _context.Infrastructures.Remove(infrastructure);
            Console.WriteLine("Infrastructure deleted: " + infrastructureName );
            await _context.SaveChangesAsync();
            return true;
        }
        
        return false;
    }
    
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}