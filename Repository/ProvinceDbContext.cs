using Microsoft.EntityFrameworkCore;
using ProvEditorNET.Models;

namespace ProvEditorNET.Repository;

public class ProvinceDbContext : DbContext
{
    public ProvinceDbContext(DbContextOptions<ProvinceDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<Province> Provinces { get; set; }
}