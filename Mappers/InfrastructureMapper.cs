using ProvEditorNET.DTO;
using ProvEditorNET.Models;

namespace ProvEditorNET.Mappers;

public static class InfrastructureMapper
{
    public static Infrastructure ToInfrastructure(this InfrastructureDto infrastructureDto)
    {
        return new Infrastructure
        {
            // TODO - change Guid.NewGuid() to Guid.CreateVersion7() once .NET 9 is released
            InfraId = Guid.NewGuid(),
            Name = infrastructureDto.InfrastructureName,
            Notes = infrastructureDto.Notes,
            Factor = infrastructureDto.Factor,
            MothlyCost = infrastructureDto.MonthlyCost,
            Price = infrastructureDto.BuildPrice,
            Technology = infrastructureDto.TechnologyLevelRequired
        };
    }

    public static InfrastructureDto ToInfrastructureDto(this Infrastructure infrastructure)
    {
        return new InfrastructureDto( 
            infrastructure.Name,
            infrastructure.Factor, 
            infrastructure.Price, 
            infrastructure.MothlyCost, 
            infrastructure.Technology, 
            infrastructure.Notes  );
    }
}