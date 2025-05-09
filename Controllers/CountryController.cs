using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProvEditorNET.DTO;
using ProvEditorNET.Helpers;
using ProvEditorNET.Interfaces;
using ProvEditorNET.Mappers;
using ProvEditorNET.Models;

namespace ProvEditorNET.Controllers;

[ApiController]
[Route($"{ApiEndpoints.ApiBase}/[controller]")]
[Authorize(Roles = "Admin,User")]   // Admin OR User - not necessarily both
public class CountriesController : ControllerBase
{
    private readonly ICountryService _countryService;
    private readonly IMeterFactory _meterFactory;
    private readonly IRedisService _redisService;

    public CountriesController(ICountryService countryService, IMeterFactory meterFactory, IRedisService redisService)
    {
        _countryService = countryService;
        _meterFactory = meterFactory;
        _redisService = redisService;
    }
    
    [HttpPost]
    public async Task<ActionResult<Country>> CreateCountry([FromBody] CountryDto countryDto, CancellationToken cancellationToken)
    {
        var country = countryDto.ToCountry();
        await _countryService.CreateAsync(country, cancellationToken);

        // Instrumentation for Prometheus/Grafana
        var meter = _meterFactory.Create("CountryAddedMeter");
        var instrument = meter.CreateCounter<int>("country_added_counter");
        instrument.Add(1);   // add 1 to this meter everytime we add Country

        // Cache invalidation
        await _redisService.InvalidateCacheAsync("country_all");
        
        return Created( $"api/v1/countries/{countryDto.CountryName}", countryDto);
    }
    
    [HttpGet]
    [ResponseCache(Duration = 120, Location = ResponseCacheLocation.Any, VaryByHeader = "Accept, Accept-Encoding")]
    // [Route("all")]
    public async Task<IActionResult> GetAllCountries(CancellationToken cancellationToken)
    {
        string redisKey = "country_all";
        
        // first check for cached entries Cached
        // await _redisService.GetSetAsync( "country_all", countryDtoList );
        var countryDtoList = await _redisService.GetAsync<IEnumerable<CountryDto>>(redisKey);
        if (countryDtoList is not null)
        {
            return Ok(countryDtoList);
        }
        else
        {
            // if not cached then fetch from DB...
            var countryList = await _countryService.GetAllCountriesAsync(cancellationToken);
            countryDtoList = countryList.Select(i => i.ToCountryDto());
            
            // ...and then cache it
            await _redisService.SetAsync(redisKey, countryDtoList);
            
            return Ok(countryDtoList);
        }
        
        // don't return List<> here (which is lazy-evaluated) cause we will get strange error about missing DbContext - misleading as fcuk
        // just return IEnumerable
    }

    [HttpGet]
    [Route("{name}")]
    [ResponseCache(Duration = 120, Location = ResponseCacheLocation.Any, VaryByHeader = "Accept, Accept-Encoding")]
    public async Task<IActionResult> GetCountryByName(string name, CancellationToken cancellationToken)
    {
        string redisKey = $"country:{name}";
        var countryDto = await _redisService.GetAsync<CountryDto>(redisKey);
        
        if (countryDto is not null)
        {
            return Ok(countryDto);
        }
        else
        {
            var country = await _countryService.GetCountryByNameAsync(name, cancellationToken);
            countryDto = country.ToCountryDto();
            
            await _redisService.SetAsync(redisKey, countryDto);
            
            return Ok(countryDto);
        }
        
    }

    [HttpPut]
    [Route("{name}")]
    public async Task<IActionResult> UpdateCountry([FromRoute] string name, [FromBody] CountryDto countryDto, CancellationToken cancellationToken)
    {
        var country = await _countryService.GetCountryByNameAsync(name, cancellationToken);

        if (country is not null)
        {
            country.Name = countryDto.CountryName;
            country.Notes = countryDto.Notes;
            await _countryService.SaveChangesAsync(cancellationToken);
            
            await _redisService.InvalidateCacheAsync("country_all");
            await _redisService.InvalidateCacheAsync( $"country:{name}" );
            
            return Ok($"Country {name} updated");
        }
        else
        {
            return NotFound("Country not found");
        }
    }
    
    [HttpDelete("name:string")]
    public async Task<IActionResult> DeleteCountry(string countryName, CancellationToken cancellationToken)
    {
        var deleted = await _countryService.DeleteCountryAsync(countryName, cancellationToken);
        
        // Cache invalidation
        await _redisService.InvalidateCacheAsync("country_all");
        
        if (deleted)
        {
            return Ok("Country deleted: " + countryName);
        }
        return BadRequest("Country not found");
    }
}