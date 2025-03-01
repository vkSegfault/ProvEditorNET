using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProvEditorNET.DTO;
using ProvEditorNET.Interfaces;
using ProvEditorNET.Models;
using ProvEditorNET.Repository;

namespace ProvEditorNET.Services;

public class ProvinceService : IProvinceService
{
    private readonly ProvinceDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IValidator<Province> _provinceValidator;
    private readonly IValidator<GetAllProvincesOptionsDto> _provincesOptionsValidator;

    public ProvinceService(ProvinceDbContext context, IIdentityService identityService, IValidator<Province> provinceValidator, IValidator<GetAllProvincesOptionsDto> provincesOptionsValidator)
    {
        _context = context;
        _identityService = identityService;
        _provinceValidator = provinceValidator;
        _provincesOptionsValidator = provincesOptionsValidator;
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
    
    public async Task<IEnumerable<Province>> GetAllProvincesAsync(GetAllProvincesOptionsDto options, CancellationToken cancellationToken = default)
    {
        await _provincesOptionsValidator.ValidateAndThrowAsync(options, cancellationToken);
        
        var provincesIncludable = _context.Provinces
            .Include(p => p.Country)
            .Include(p => p.Infrastructures)
            .Include(p => p.Resources);

        
        // TODO
        // this condition based Collection building is hacky as fuck - refactor it to something else
        IQueryable<Province> provincesQueryable = null;
        
        // ORDERING of filters matters - if we first e.g.: limit number of results we won't check .StartsWith() on all collection
        // .limit() should be last
        if (!string.IsNullOrEmpty(options.limitToCountry))
        {
            if (provincesQueryable != null)
            {
                provincesQueryable = provincesQueryable.Where(p => p.Country.Name == options.limitToCountry);
            }
            else
            {
                provincesQueryable = provincesIncludable.Where(p => p.Country.Name == options.limitToCountry);
            }
        }

        if (!string.IsNullOrEmpty(options.startsWith))
        {
            if (provincesQueryable != null)
            {
                provincesQueryable = provincesQueryable.Where(p => p.Name.StartsWith(options.startsWith));
            }
            else
            {
                provincesQueryable = provincesIncludable.Where(p => p.Name.StartsWith(options.startsWith));
            }
        }

        if (options.pageSize != 0)
        {
            if (provincesQueryable != null)
            {
                provincesQueryable = provincesQueryable.Take(options.pageSize);
            }
            else
            {
                provincesQueryable = provincesIncludable.Take(options.pageSize);
            }
        }
        
        if (options.sortByName)
        {
            if (provincesQueryable != null)
            {
                provincesQueryable = provincesQueryable.OrderBy(p => p.Name);
            }
            else
            {
                provincesQueryable = provincesIncludable.OrderBy(p => p.Name);
            }
        }
        
        if (provincesQueryable != null)   // if any limits applied
        {
            var provinces = await provincesQueryable
                .OrderByDescending(p => p.Name)
                .ToListAsync(cancellationToken);
            return provinces;
        }
        else   // no limits applied
        {
            var provinces = await provincesIncludable
                .OrderByDescending(p => p.Name)
                .ToListAsync(cancellationToken);
            return provinces;
        }
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