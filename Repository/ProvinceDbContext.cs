using Microsoft.EntityFrameworkCore;
using ProvEditorNET.Models;

namespace ProvEditorNET.Repository;

public class ProvinceDbContext : DbContext
{
    public ProvinceDbContext(DbContextOptions<ProvinceDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<Province> Provinces { get; set; }
    public DbSet<Country> Countries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Country has many Provinces
        modelBuilder.Entity<Country>()
            .HasMany(c => c.Provinces)
            .WithOne(p => p.Country)
            .HasForeignKey(p => p.CountryId)
            .HasPrincipalKey(c => c.CountryId);

        // make Province Name Unique Constraint
        modelBuilder.Entity<Province>()
            .HasAlternateKey(p => p.Name);
        
        // make Country Name Unique Constraint
        modelBuilder.Entity<Country>()
            .HasAlternateKey(c => c.Name);
        
        // Province has many Resources
        modelBuilder.Entity<Province>()
            .HasMany(p => p.Resources)
            .WithOne(r => r.Province)
            .HasForeignKey(r => r.ProvinceId)
            .HasPrincipalKey(p => p.ProvinceId);
    }
}