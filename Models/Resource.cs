using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProvEditorNET.Models;

[Table("resource")]
public class Resource : DbContext
{
    [Key]
    [Column("resourceId")]
    public Guid ResourceId { get; set; }
    
    [Required]
    [Column("resourceName")]
    public string Name { get; set; } = string.Empty;
    
    // TODO - should be there base price of resource or should it be inferred from the world supply?

    [Column("notes")]
    public string Notes { get; set; } = string.Empty;

    public List<Province> Provinces { get; set; } = [];
    // public ICollection<ProvinceResource> ProvinceResources { get; set; }  // "Skip Navigation" used by EF Core to create Joint Entity to map many-to-many

    // navigation key
    // public Guid ProvinceId { get; set; }
    // public Province Province { get; set; }
}