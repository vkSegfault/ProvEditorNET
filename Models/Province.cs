using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProvEditorNET.Models;

[Table("province")]
public class Province : DbContext
{
    [Key]
    [Column("provinceId")]
    public Guid ProvinceId { get; set; }
    
    [Required]
    [Column("provinceName")]
    public string Name { get; set; } = string.Empty;
    
    // Country can't be required because Sea Provinces can't be part of country
    public Guid CountryId { get; set; }
    public Country Country { get; set; }

    [Required]
    [Column("population")]
    public int Population { get; set; }

    // navigation key
    // public Guid ResourceId { get; set; }
    // public Resource Resource { get; set; }

    public List<Resource> Resources { get; set; } = [];
    // public ICollection<ProvinceResource> ProvinceResources { get; set; }  // "Skip Navigation" used by EF Core to create Joint Entity to map many-to-many

}