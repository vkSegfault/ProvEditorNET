using ProvEditorNET.DTO;
using ProvEditorNET.Models;
using ProvEditorNET.Services;

namespace ProvEditorNET.Mappers;

// static class shouldn't use DI
public static class ProvinceMapper
{
    
    public static Province ToProvince(this ProvinceDto provinceDto, Country country)
    {
        return new Province
        {
            // TODO - change Guid.NewGuid() to Guid.CreateVersion7() once .NET 9 is released
            ProvinceId = Guid.NewGuid(),
            Name = provinceDto.ProvinceName,
            Country = country,
            Population = provinceDto.Population,
        };
    }
}