using ProvEditorNET.DTO;
using ProvEditorNET.Models;

namespace ProvEditorNET.Mappers;

public static class ResourceMapper
{
    public static Resource ToResource(this ResourceDto resourceDto)
    {
        return new Resource
        {
            // TODO - change Guid.NewGuid() to Guid.CreateVersion7() once .NET 9 is released
            ResourceId = Guid.NewGuid(),
            Name = resourceDto.ResourceName,
            Notes = resourceDto.Notes,
        };
    }

    public static ResourceDto ToResourceDto(this Resource resource)
    {
        return new ResourceDto( resource.Name, resource.Notes );
    }
}