using ProvEditorNET.Models;

namespace ProvEditorNET.Interfaces;

public interface IProvinceService
{
    Task CreateAsync(Province province);
    Task<IEnumerable<Province>> GetAllProvincesAsync();
    Task<Province> GetProvinceByNameAsync(string provinceName);
    Task<bool> DeleteProvinceAsync(string provinceName);
}