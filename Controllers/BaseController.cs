using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProvEditorNET.Repository;

namespace ProvEditorNET.Controllers;

public class BaseController : ControllerBase
{
    private readonly ProvinceDbContext _context;

    public BaseController(ProvinceDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    [Route("applymigrations")]
    public async Task<IActionResult> GetAllCountries()
    {
        var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();

        if ( pendingMigrations.Any() )
        {
            Console.WriteLine($"Pending migrations: {pendingMigrations.Count()}");
            await _context.Database.MigrateAsync();
            await _context.Database.EnsureCreatedAsync();
        }
        
        return Ok("Migrations applied");
    }
}