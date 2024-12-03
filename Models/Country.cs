using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProvEditorNET.Models;

[Table("country")]
public class Country : DbContext
{
    [Key]
    [Column("countryId")]
    public Guid Id { get; set; }
    
    [Required]
    [Column("countryName")]
    public string Name { get; set; } = string.Empty;
    
    public ICollection<Province> Provinces { get; }
}