using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProvEditorNET.DTO;
using ProvEditorNET.Interfaces;
using ProvEditorNET.Mappers;
using ProvEditorNET.Models;

namespace ProvEditorNET.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "Admin,User")]   // Admin OR User - not necessarily both
public class CountryController : ControllerBase
{
    private readonly ICountryService _countryService;
    private readonly IMeterFactory _meterFactory;

    public CountryController(ICountryService countryService, IMeterFactory meterFactory)
    {
        _countryService = countryService;
        _meterFactory = meterFactory;
    }
    
    [HttpPost]
    public async Task<ActionResult<Country>> CreateCountry([FromBody] CountryDto countryDto)
    {
        var country = countryDto.ToCountry();
        await _countryService.CreateAsync(country);
        
        var meter = _meterFactory.Create("CountryAddedMeter");
        var instrument = meter.CreateCounter<int>("country_added_counter");
        instrument.Add(1);   // add 1 to this meter everytime we add Country

        return Ok();
    }
    
    [HttpGet]
    [Route("all")]
    public async Task<IActionResult> GetAllCountries()
    {
        var countryList = await _countryService.GetAllCountriesAsync();
        var countryDtoList = countryList.Select(i => i.ToCountryDto());
        
        // don't return List<> here (which is lazy-evaluated) cause we will get strange error about missing DbContext - misleading as fcuk
        // just return IEnumerable
        return Ok(countryDtoList);
    }

    [HttpGet]
    [Route("{name}")]
    public async Task<IActionResult> GetCountryByName(string name)
    {
        var country = await _countryService.GetCountryByNameAsync(name);
        var countryDto = country.ToCountryDto();
        return Ok(countryDto);
    }
    
    
    [HttpDelete("name:string")]
    public async Task<IActionResult> DeleteCountry(string countryName)
    {
        var deleted = await _countryService.DeleteCountryAsync(countryName);
        if (deleted)
        {
            return Ok("Country deleted: " + countryName);
        }
        return BadRequest("Country not found");
    }
}