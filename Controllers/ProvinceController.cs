using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using ProvEditorNET.DTO;
using ProvEditorNET.Interfaces;
using ProvEditorNET.Mappers;
using ProvEditorNET.Models;

namespace ProvEditorNET.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ProvinceController: ControllerBase
{
    private readonly IProvinceService _provinceService;
    private readonly ICountryService _countryService;
    private readonly IResourceService _resourceService;
    private readonly IInfrastructureService _infrastructureService;

    public ProvinceController(IProvinceService provinceService, ICountryService countryService, IResourceService resourceService, IInfrastructureService infrastructureService)
    {
        _provinceService = provinceService;
        _countryService = countryService;
        _resourceService = resourceService;
        _infrastructureService = infrastructureService;
    }
    
    
    // TODO - move this healthcheck to seperate controller
    // TODO - add more checks like db connection check, quick services functionallity check etc
    [HttpGet("healthcheck", Name = "Healthcheck")]
    [AllowAnonymous]   // make this endpoint accessible without authorization 
    public async Task<ActionResult<Boolean>> Healthcheck()
    {
        return Ok(true);
    }
    
    
    [HttpPost]
    public async Task<ActionResult<Province>> CreateProvince([FromBody] ProvinceRequestDto provinceRequestDto)
    {
        Country country = await _countryService.GetCountryByNameAsync(provinceRequestDto.CountryName);
        if (country is null)
        {
            return NotFound("Country not found");
        }

        ICollection<Resource> resources;
        try
        {
            resources = await _resourceService.GetResourcesFromStringListAsync( provinceRequestDto.Resources );
        }
        catch (Exception e)
        {
            return BadRequest("Resource not found: " + e.Message);
        }
        
        ICollection<Infrastructure> infrastructures;
        try
        {
            infrastructures = await _infrastructureService.GetInfrastructuresFromStringListAsync( provinceRequestDto.Infrastructures );
        }
        catch (Exception e)
        {
            return BadRequest("Infrastructure not found: " + e.Message);
        }
        
        var province = provinceRequestDto.ToProvince(country, resources, infrastructures);
        
        (bool success, string msg) created = await _provinceService.CreateAsync(province);

        if ( created.success )
        {
            return Ok("Province created: " + province.Name);
        }
        else
        {
            return BadRequest("Province creation failed: " + province.Name + " --> " + created.msg);
        }
    }
    
    
    [HttpGet]   // adding [HttpGet(Name = "GetProvinces")] will fail on Swagger creation
    [Route("all")]
    public async Task<ActionResult<List<String>>> GetAllProvinces()
    {
        var provinces = await _provinceService.GetAllProvincesAsync();
        var provincesDto = provinces.Select(p => p.ToProvinceRequestDto());

        // TODO - remove thread sleep - just used for testing frontend delays
        Thread.Sleep(2000);
        return Ok(provincesDto);
    }
    
    
    [HttpGet]
    [Route("{name}")]
    public async Task<IActionResult> GetProvince([FromRoute] string name)
    {
        var province = await _provinceService.GetProvinceByNameAsync(name);
        if (province != null)
        {
            var provinceDto = province.ToProvinceRequestDto();
            
            // TODO - remove thread sleep - just used for testing frontend delays
            Thread.Sleep(2000);
            return Ok(provinceDto);
        }
        else
        {
            return NotFound();
        }

    }
    
    
    // TODO
    // PUT update endpoint
    [HttpPut("{name}")]
    public async Task<IActionResult> UpdateProvince([FromRoute] string name, [FromQuery] string newCountryName, [FromQuery] int newPopulation)
    {
        var province = await _provinceService.GetProvinceByNameAsync(name);
        if ( province is not null )
        {
            // to update an Province.Name which is key we need to first strip of relations --> remove Country, then chnage name --> readd Country
            // [FromQuery] string newProvinceName,
            // if (newProvinceName is not null)
            // {
            //     Country country = province.Country;
            //     // TODO
            //     // here happens Cascading Delete ==> when parent (here Country) is deleted or child references required relation (here Province references Country) child is deleted by default
            //      https://learn.microsoft.com/en-us/ef/core/saving/cascade-delete
            //     // province.Country = null;   // remove dependent here
            //     // await _provinceService.SaveChangesAsync();
            //     
            //     province.Name = newProvinceName;
            //     province.Country = country;   // readd dependent
            //     
            //     await _provinceService.SaveChangesAsync();
            // }
            
            if (newCountryName is not null)
            {
                Country country = await _countryService.GetCountryByNameAsync(newCountryName);
                if (country is not null)
                {
                    province.Country = country;
                }
            }

            if (newPopulation > 0)   // default for int (if not provided) is 0
            {
                province.Population = newPopulation;
            }

            await _provinceService.SaveChangesAsync();
            return NoContent();
        }
        else
        {
            return NotFound("Province not found");
        }

    }
    
    
    [HttpDelete]
    public async Task<IActionResult> DeleteProvince(string provinceName)
    {
        var deleted = await _provinceService.DeleteProvinceAsync(provinceName);
        if (deleted)
        {
            return Ok("Province deleted: " + provinceName);
        }
        return BadRequest("Province not found");
    }
    
    // this endpoint is secured
    // (1 --------------- Bearer Access Token) go to /login endpoint and pass your login and password:
    // curl -X 'POST' 'http://localhost:5077/login' -H 'accept: application/json' -H 'Content-Type: application/json' -d '{
    //   "email": "yourmail@mail.com",
    //   "password": "YourPassword",
    //   "twoFactorCode": "string",
    //   "twoFactorRecoveryCode": "string"
    // }'
    // we will get access token upon succesful login, copy it
    // to any secured endpoint just add this token like so: -H 'accept: text/plain' -H 'Authorization: Bearer OMx_36NgZOqRSICWKSLRGGF'
    // (2 ------------- Cookies - instead copying token manually it's saved as cookie in browser cache and we don't need to pass it anywhere), go to /login and pass your creds:
    // curl -X 'POST' 'http://localhost:5077/login?useCookies=true'' -H 'accept: application/json' -H 'Content-Type: application/json' -d '{
    //   "email": "yourmail@mail.com",
    //   "password": "YourPassword",
    //   "twoFactorCode": "string",
    //   "twoFactorRecoveryCode": "string"
    // }'
    // we can now access any secured endpoint

}