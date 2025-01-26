using ProvEditorNET.Models;

namespace ProvEditorNET.Interfaces;

public interface IResourceService
{
    Task CreateAsync(Resource resource);
    Task<IEnumerable<Resource>> GetAllResourcesAsync();
    Task<Resource> GetResourceByNameAsync(string resourceName);
    Task<ICollection<Resource>> GetResourcesFromStringListAsync(IEnumerable<string> resourceNames);
    Task<bool> DeleteResourceAsync(string resourceName);
}