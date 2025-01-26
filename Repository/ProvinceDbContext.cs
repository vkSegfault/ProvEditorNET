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
    public DbSet<Resource> Resources { get; set; }
    // public DbSet<ProvinceResource> ProvinceResources { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Country has many Provinces (one to many)
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
        
        // many Provinces have many Resources (many to many)
        modelBuilder.Entity<Province>()
            .HasMany(p => p.Resources)
            .WithMany(r => r.Provinces);
        // .UsingEntity(
        //     "ProvinceResource",
        //     l => l.HasOne(typeof(Resource)).WithMany().HasForeignKey("ResourcesId").HasPrincipalKey(nameof(Resource.ResourceId)),
        //     r => r.HasOne(typeof(Province)).WithMany().HasForeignKey("ProvincesId").HasPrincipalKey(nameof(Province.ProvinceId)),
        //     j => j.HasKey("ProvincesId", "ResourcesId")
        //     );
        // .HasForeignKey(r => r.ProvinceId)
        // .HasPrincipalKey(p => p.ProvinceId);
    }
}