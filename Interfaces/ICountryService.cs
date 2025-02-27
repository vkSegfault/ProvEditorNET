using ProvEditorNET.Models;

namespace ProvEditorNET.Interfaces;

public interface ICountryService
{
    Task CreateAsync(Country country, CancellationToken cancellationToken = default);
    Task<IEnumerable<Country>> GetAllCountriesAsync(CancellationToken cancellationToken = default);
    Task<Country> GetCountryByNameAsync(string countryName, CancellationToken cancellationToken = default);
    Task<bool> DeleteCountryAsync(string countryName, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}