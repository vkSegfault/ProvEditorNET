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
    
}