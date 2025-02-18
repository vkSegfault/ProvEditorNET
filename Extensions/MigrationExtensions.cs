using Microsoft.EntityFrameworkCore;
using ProvEditorNET.Repository;

namespace ProvEditorNET.Extensions;

public static class MigrationExtensions
{
    public async static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope serviceScope = app.ApplicationServices.CreateScope();

        await using ProvinceDbContext dbContext = serviceScope.ServiceProvider.GetRequiredService<ProvinceDbContext>();
        
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

        foreach (var migration in pendingMigrations)
        {
            Console.WriteLine(migration);
        }
        
        if ( pendingMigrations.Any() )
        {
            Console.WriteLine($"Pending migrations: {pendingMigrations.Count()}");
            await dbContext.Database.MigrateAsync();
            await dbContext.Database.EnsureCreatedAsync();
        }
    }
}