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

    public ProvinceController(IProvinceService provinceService, ICountryService countryService)
    {
        _provinceService = provinceService;
        _countryService = countryService;
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
    public async Task<ActionResult<Province>> CreateProvince([FromBody] ProvinceDto provinceDto)
    {
        Country country = await _countryService.GetCountryByName(provinceDto.CountryName);
        if (country is null)
        {
            return NotFound("Country not found");
        }
        var province = provinceDto.ToProvince(country);
        await _provinceService.CreateAsync(province);

        return Ok("Province created: " + province.Name);
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
    [HttpGet(Name = "GetProvinces")]
    [Authorize]
    public async Task<ActionResult<List<String>>> GetAllProvinces()
    {
        var provinces = new List<String>{ "pomerania", "masovia" };
        provinces.Add("silesia");

        // TODO - remove thread sleep - just used for testing frontend delays
        Thread.Sleep(2000);
        return Ok(provinces);
    }
}