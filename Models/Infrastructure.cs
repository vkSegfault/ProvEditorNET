using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProvEditorNET.Models;

[Table("infrastructure")]
public class Infrastructure : DbContext
{
    [Key]
    [Column("infraId")]
    public Guid InfraId { get; set; }
    
    [Required]
    [Column("infraName")]
    public string Name { get; set; } = string.Empty;

    // how much general level of infrastructure will grow once this infra object is built
    [Required]
    [Column("infraFactor")]
    public int Factor { get; set; }

    // cost in money (note that actual price should be multiplied by inflation level)
    [Required]
    [Column("infraPrice")]
    public int Price { get; set; }

    // we pay motnhly some amount of money for that object to operate/maintain
    [Required]
    [Column("infraMonthlyCost")]
    public int MothlyCost { get; set; }

    // to build particular infra object we need enough technology
    // TODO - decide whether is should be some 1-10 level or some list of string-named technologies "Nuclear Knowledge"
    [Required]
    [Column("technologyRequired")]
    public int Technology { get; set; }
    
    [Column("notes")]
    public string Notes { get; set; } = string.Empty;
    
    // Navigation for EF Core
    public List<Province> Provinces { get; set; } = [];
}