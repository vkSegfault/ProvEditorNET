using ProvEditorNET.Models;

namespace ProvEditorNET.Interfaces;

public interface IResourceService
{
    Task CreateAsync(Resource resource);
    Task<IEnumerable<Resource>> GetAllResourcesAsync();
    Task<Resource> GetResourceByNameAsync(string resourceName);
    Task<bool> DeleteResourceAsync(string resourceName);
}