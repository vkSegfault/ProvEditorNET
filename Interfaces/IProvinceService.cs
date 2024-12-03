using ProvEditorNET.Models;

namespace ProvEditorNET.Interfaces;

public interface IProvinceService
{
    Task CreateAsync(Province province);
}