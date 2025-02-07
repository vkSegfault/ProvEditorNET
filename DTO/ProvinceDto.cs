namespace ProvEditorNET.DTO;

public record ProvinceDto(
    string ProvinceType,
    string ProvinceName,
    string CountryName,
    string Notes,
    List<float> Shape,
    int Population,
    IEnumerable<string> Resources,
    IEnumerable<string> Infrastructures
    );