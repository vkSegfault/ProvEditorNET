using ProvEditorNET.DTO;
using ProvEditorNET.Models;
using ProvEditorNET.Services;

namespace ProvEditorNET.Mappers;

// static class shouldn't use DI
public static class ProvinceMapper
{
    
    public static Province ToProvince(this CreateProvinceRequestDto provinceRequestDto, Country country, ICollection<Resource> resources, ICollection<Infrastructure> infrastructure)
    {
        return new Province
        {
            // TODO - change Guid.NewGuid() to Guid.CreateVersion7() once .NET 9 is released
            ProvinceId = Guid.NewGuid(),
            AuthoredBy = String.Empty,
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = String.Empty,
            ModifiedDate = DateTime.UtcNow,
            ProvinceType = provinceRequestDto.ProvinceType,
            Name = provinceRequestDto.ProvinceName,
            Notes = provinceRequestDto.Notes,
            Country = country,
            Shape = provinceRequestDto.Shape,
            Population = provinceRequestDto.Population,
            Resources = resources.ToList(),
            Infrastructures = infrastructure.ToList()
        };
    }

    public static CreateProvinceRequestDto ToProvinceRequestDto(this Province province)
    {
        ICollection<string> resourcesStr = new List<string>();
        foreach (var resource in province.Resources)
        {
            resourcesStr.Add( resource.Name );
        }
        
        ICollection<string> infrastructuresStr = new List<string>();
        foreach (var infrastructure in province.Infrastructures)
        {
            infrastructuresStr.Add( infrastructure.Name );
        }
        
        return new CreateProvinceRequestDto( 
            province.ProvinceType,
            province.Name, 
            province.Country.Name, 
            province.Notes,
            province.Shape,
            province.Population,
            resourcesStr,
            infrastructuresStr
            );
    }
    
    public static ProvinceResponseDto ToProvinceResponseDto(this Province province)
    {
        ICollection<string> resourcesStr = new List<string>();
        foreach (var resource in province.Resources)
        {
            resourcesStr.Add( resource.Name );
        }
        
        ICollection<string> infrastructuresStr = new List<string>();
        foreach (var infrastructure in province.Infrastructures)
        {
            infrastructuresStr.Add( infrastructure.Name );
        }
        
        return new ProvinceResponseDto( 
            province.AuthoredBy,
            province.CreatedDate,
            province.ModifiedBy,
            province.ModifiedDate,
            province.ProvinceType,
            province.Name, 
            province.Country.Name, 
            province.Notes,
            province.Shape,
            province.Population,
            resourcesStr,
            infrastructuresStr
        );
    }
}