using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProvEditorNET.Interfaces;

namespace ProvEditorNET.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class CountryController : ControllerBase
{
    private readonly ICountryService _countryService;

    public CountryController(ICountryService countryService)
    {
        _countryService = countryService;
    }
    
    
}