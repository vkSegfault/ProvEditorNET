using ProvEditorNET.Models;

namespace ProvEditorNET.Interfaces;

public interface ICountryService
{
    Task CreateAsync(Country country);
    Task<IEnumerable<Country>> GetAllCountriesAsync();
    Task<Country> GetCountryByNameAsync(string countryName);
    Task<bool> DeleteCountryAsync(string countryName);
}