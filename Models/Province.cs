using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace ProvEditorNET.Models;

[Table("province")]
public class Province : DbContext
{
    [Key]
    [Column("provinceId")]
    public Guid ProvinceId { get; set; }
    
    [Required]
    [Column("provinceType")]
    [MaxLength(10)]
    public ProvinceType ProvinceType { get; set; }
    
    [Required]
    [Column("provinceName")]
    [MaxLength(30)]
    public string Name { get; set; } = string.Empty;
    
    // Country can't be required because Sea Provinces can't be part of country
    public Guid CountryId { get; set; }
    public Country Country { get; set; }

    [Column("notes")]
    public string Notes { get; set; } = string.Empty;
    
    // type Point comes from Npgsql.EntityFrameworkCore.PostgreSQL.NetTopologySuite
    // Postgres DB needs extension on it's side: CREATE EXTENSION postgis;
    [Required]
    [Column("shape")]
    public Point Shape { get; set; }
    
    [Required]
    [Column("population")]
    public int Population { get; set; }

    // navigation key
    // public Guid ResourceId { get; set; }
    // public Resource Resource { get; set; }

    public List<Resource> Resources { get; set; } = [];
    // public ICollection<ProvinceResource> ProvinceResources { get; set; }  // "Skip Navigation" used by EF Core to create Joint Entity to map many-to-many
    
    public List<Infrastructure> Infrastructures { get; set; } = [];
}

public enum ProvinceType : ushort
{
    Land = 0,
    City = 1,
    Sea = 2
}