using ProvEditorNET.DTO;
using ProvEditorNET.Models;
using ProvEditorNET.Services;

namespace ProvEditorNET.Mappers;

// static class shouldn't use DI
public static class ProvinceMapper
{
    
    public static Province ToProvince(this ProvinceDto provinceDto, Country country, ICollection<Resource> resources)
    {
        return new Province
        {
            // TODO - change Guid.NewGuid() to Guid.CreateVersion7() once .NET 9 is released
            ProvinceId = Guid.NewGuid(),
            Name = provinceDto.ProvinceName,
            Country = country,
            Population = provinceDto.Population,
            Resources = resources.ToList()
        };
    }

    public static ProvinceDto ToProvinceDto(this Province province)
    {
        ICollection<string> resourcesStr = new List<string>();
        foreach (var resource in province.Resources)
        {
            resourcesStr.Add( resource.Name );
        }
        return new ProvinceDto( 
            province.Name, 
            province.Country.Name, 
            province.Population,
            resourcesStr
            );
    }
}