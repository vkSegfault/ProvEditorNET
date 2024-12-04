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

    public CountryController(ICountryService countryService)
    {
        _countryService = countryService;
    }
    
    [HttpPost]
    public async Task<ActionResult<Country>> CreateCountry([FromBody] CountryDto countryDto)
    {
        var country = countryDto.ToCountry();
        await _countryService.CreateAsync(country);

        return Ok();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllCountries()
    {
        var countryList = await _countryService.GetAllCountriesAsync();
        var countryDtoList = countryList.Select(i => i.ToCountryDto());

        // don't return List<> here (which is lazy-evaluated) cause we will get strange error about missing DbContext - misleading as fcuk
        // just return IEnumerable
        return Ok(countryDtoList);
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