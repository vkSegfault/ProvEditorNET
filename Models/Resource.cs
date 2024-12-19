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
}