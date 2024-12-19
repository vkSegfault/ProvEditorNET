using ProvEditorNET.DTO;
using ProvEditorNET.Models;
using ProvEditorNET.Services;

namespace ProvEditorNET.Mappers;

// static class shouldn't use DI
public static class ProvinceMapper
{
    
    public static Province ToProvince(this ProvinceDto provinceDto, Country country)
    {
        ICollection<Resource> resources = new List<Resource>();
        foreach (var resurce in provinceDto.Resources)
        {
            // we can't use DI so no Resource Service to use...
            var resource = 
        }
        
        return new Province
        {
            // TODO - change Guid.NewGuid() to Guid.CreateVersion7() once .NET 9 is released
            ProvinceId = Guid.NewGuid(),
            Name = provinceDto.ProvinceName,
            Country = country,
            Population = provinceDto.Population,
            Resources = provinceDto.Resources
        };
    }

    public static ProvinceDto ToProvinceDto(this Province province)
    {
        return new ProvinceDto( 
            province.Name, 
            province.Country.Name, 
            province.Population,
            province.Resources
            );
    }
}