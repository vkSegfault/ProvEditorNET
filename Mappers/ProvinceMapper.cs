using ProvEditorNET.DTO;
using ProvEditorNET.Models;

namespace ProvEditorNET.Mappers;

public static class ProvinceMapper
{
    public static Province ToProvince(this ProvinceDto provinceDto)
    {
        return new Province
        {
            // TODO - change Guid.NewGuid() to Guid.CreateVersion7() once .NET 9 is released
            ProvinceId = Guid.NewGuid(),
            Name = provinceDto.ProvinceName,
        };
    }
}