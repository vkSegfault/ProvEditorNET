using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProvEditorNET.Interfaces;
using ProvEditorNET.Models;
using ProvEditorNET.Repository;

namespace ProvEditorNET.Services;

public class ProvinceService : IProvinceService
{
    private readonly ProvinceDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IValidator<Province> _provinceValidator;

    public ProvinceService(ProvinceDbContext context, IIdentityService identityService, IValidator<Province> provinceValidator)
    {
        _context = context;
        _identityService = identityService;
        _provinceValidator = provinceValidator;
    }

    public async Task<(bool success, string msg)> CreateAsync(Province province)
    {
        Console.WriteLine("Called ProvinceService.CreateAsync");

        province.AuthoredBy = _identityService.GetLoggedInUsername();
        province.ModifiedBy = _identityService.GetLoggedInUsername();
        
        try
        {
            await _provinceValidator.ValidateAndThrowAsync(province);   // should go to UPDATE function as well
            await _context.Provinces.AddAsync(province);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            if (e is DbUpdateException)
            {
                var message = e.GetBaseException().Message;
                Console.WriteLine("==> Exception saving Province: " + message);
                return (false, message);
            }
            else
            {
                Console.WriteLine("==> Exception saving Province: " + e.Message);
                return (false, e.Message);
            }
        }
        return (true, "success");
    }
    
    public async Task<IEnumerable<Province>> GetAllProvincesAsync()
    {
        var provinces = await _context.Provinces
            .Include(p => p.Country)
            .Include(p => p.Infrastructures)
            .Include(p => p.Resources)
            .ToListAsync();
        return provinces;
    }

    public async Task<Province> GetProvinceByNameAsync(string provinceName)
    {
        // .Include() does eager loading of Country relation as a part of Province query to DB
        // https://learn.microsoft.com/en-us/ef/core/querying/related-data/
        var province = await _context.Provinces
            .Include(p => p.Country)
            .Include(p => p.Resources)
            .Include(p => p.Infrastructures)
            .FirstOrDefaultAsync(x => x.Name == provinceName);
        
        return province;
    }
    
    public async Task<bool> DeleteProvinceAsync(string provinceName)
    {
        var province = await _context.Provinces.FirstOrDefaultAsync(x => x.Name == provinceName);
        if (province != null)
        {
            _context.Provinces.Remove(province);
            Console.WriteLine("Province deleted: " + provinceName );
            await _context.SaveChangesAsync();
            return true;
        }
        
        return false;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}