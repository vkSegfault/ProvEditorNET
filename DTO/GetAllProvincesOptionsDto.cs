namespace ProvEditorNET.DTO;

public record GetAllProvincesOptionsDto(
    int limit = 0,
    string limitToCountry = null,
    string startsWith = null,
    bool sortByName = false   //ASC for name, DESC for -name
    );