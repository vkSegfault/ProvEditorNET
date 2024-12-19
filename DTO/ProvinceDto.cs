namespace ProvEditorNET.DTO;

public record ProvinceDto( 
    string ProvinceName, 
    string CountryName,
    int Population,
    ICollection<string> Resources
    );