using ProvEditorNET.Models;

namespace ProvEditorNET.Interfaces;

public interface IInfrastructureService
{
    Task CreateAsync(Infrastructure infrastructure);
    Task<IEnumerable<Infrastructure>> GetAllInfrastrcturesAsync();
    Task<Infrastructure> GetInfrastructureByNameAsync(string infrastructureName);
    Task<bool> DeleteAsync(string infrastrcureName);
}