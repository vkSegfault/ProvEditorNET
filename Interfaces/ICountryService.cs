using ProvEditorNET.Models;

namespace ProvEditorNET.Interfaces;

public interface ICountryService
{
    Task CreateAsync(Country country);
}