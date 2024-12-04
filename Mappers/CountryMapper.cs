using ProvEditorNET.DTO;
using ProvEditorNET.Models;

namespace ProvEditorNET.Mappers;

public static class CountryMapper
{
    public static Country ToCountry(this CountryDto countryDto)
    {
        return new Country
        {
            // TODO - change Guid.NewGuid() to Guid.CreateVersion7() once .NET 9 is released
            CountryId = Guid.NewGuid(),
            Name = countryDto.CountryName,
        };
    }

    public static CountryDto ToCountryDto(this Country country)
    {
        return new CountryDto( country.Name );
    }
}