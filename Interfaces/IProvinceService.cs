using ProvEditorNET.DTO;
using ProvEditorNET.Models;

namespace ProvEditorNET.Interfaces;

public interface IProvinceService
{
    Task<(bool success, string msg)> CreateAsync(Province province);
    Task<IEnumerable<Province>> GetAllProvincesAsync(GetAllProvincesOptionsDto options, CancellationToken cancellationToken = default);
    Task<Province> GetProvinceByNameAsync(string provinceName);
    Task<bool> DeleteProvinceAsync(string provinceName);
    
    Task SaveChangesAsync();
}