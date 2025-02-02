namespace ProvEditorNET.DTO;

public record InfrastructureDto(
    string InfrastructureName,
    int Factor,   // factor how much we would increase infra level in province
    int BuildPrice,
    int MonthlyCost,
    int TechnologyLevelRequired,   // this should be list of string technologies names instead
    string Notes 
    );