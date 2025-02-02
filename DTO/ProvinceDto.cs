using NetTopologySuite.Geometries;

namespace ProvEditorNET.DTO;

public record ProvinceDto(
    string ProvinceType,
    string ProvinceName,
    string CountryName,
    string Notes,
    Point Shape,
    int Population,
    IEnumerable<string> Resources,
    IEnumerable<string> Infrastructures
    );