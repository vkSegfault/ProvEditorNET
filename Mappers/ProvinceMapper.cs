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
            ProvinceType = provinceDto.ProvinceType,
            Name = provinceDto.ProvinceName,
            Notes = provinceDto.Notes,
            Country = country,
            Population = provinceDto.Population,
            Resources = resources.ToList(),
            Infrastructures = provinceDto.Infrastructures.ToList()
        };
    }

    public static ProvinceDto ToProvinceDto(this Province province)
    {
        ICollection<string> resourcesStr = new List<string>();
        foreach (var resource in province.Resources)
        {
            resourcesStr.Add( resource.Name );
        }
        
        ICollection<string> infrastructursStr = new List<string>();
        // TODO - call fetch infra list like with resources above
        
        return new ProvinceDto( 
            Enum.GetName(province.ProvinceType),
            province.Name, 
            province.Country.Name, 
            province.Notes,
            province.Shape,
            province.Population,
            resourcesStr,
            infrastructureStr
            );
    }
}