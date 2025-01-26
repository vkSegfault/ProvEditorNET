using Microsoft.EntityFrameworkCore;

namespace ProvEditorNET.Models;

public class ProvinceResource : DbContext
{
    public Guid ProvincesId { get; set; }
    public Guid ResourcesId { get; set; }
    public Province Province { get; set; } = null!;
    public Resource Resource { get; set; } = null!;
}