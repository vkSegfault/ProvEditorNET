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

    public ICollection<Resource> Resources { get; set; }
}