namespace ProvEditorNET.DTO;

public record ProvinceResponseDto(
    string AuthoredBy,
    DateTime CreatedAt,
    string ModifiedBy,
    DateTime ModifiedAt,
    string ProvinceType,
    string ProvinceName,
    string CountryName,
    string Notes,
    List<float> Shape,
    int Population,
    IEnumerable<string> Resources,
    IEnumerable<string> Infrastructures
    );