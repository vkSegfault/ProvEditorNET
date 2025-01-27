using Microsoft.EntityFrameworkCore;
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
        await _context.Resources.AddAsync(resource);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Resource>> GetAllResourcesAsync()
    {
        var resources = await _context.Resources.ToListAsync();
        return resources;
    }

    public async Task<Resource> GetResourceByNameAsync(string resourceName)
    {
        var resource = await _context.Resources.FirstOrDefaultAsync(x => x.Name == resourceName);

        if (resource is null)
        {
            throw new NullReferenceException("Resource not found: " + resourceName);
        }
        return resource;
    }

    public async Task<ICollection<Resource>> GetResourcesFromStringListAsync(IEnumerable<string> resourceNames)
    {
        ICollection<Resource> resources = new List<Resource>();
        foreach (var resourceName in resourceNames)
        {
            Resource resource = await GetResourceByNameAsync(resourceName);
            resources.Add( resource );
        }
        
        return resources;
    }
    
    public async Task<bool> DeleteResourceAsync(string resourceName)
    {
        var resource = await _context.Resources.FirstOrDefaultAsync(x => x.Name == resourceName);
        if (resource != null)
        {
            _context.Resources.Remove(resource);
            Console.WriteLine("Country deleted: " + resourceName );
            await _context.SaveChangesAsync();
            return true;
        }
        
        return false;
    }
}