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
public class InfrastructureController : ControllerBase
{
    private readonly IInfrastructureService _infrastructureService;
    private readonly IMeterFactory _meterFactory;

    public InfrastructureController(IInfrastructureService infrastructureService, IMeterFactory meterFactory)
    {
        _infrastructureService = infrastructureService;
        _meterFactory = meterFactory;
    }
    
    [HttpPost]
    public async Task<ActionResult<Infrastructure>> CreateInfrastructure([FromBody] InfrastructureDto infrastructureDto)
    {
        var infra = infrastructureDto.ToInfrastructure();
        await _infrastructureService.CreateAsync(infra);
        
        var meter = _meterFactory.Create("InfrastructureAddedMeter");
        var instrument = meter.CreateCounter<int>("country_added_counter");
        instrument.Add(1);   // add 1 to this meter everytime we add Country

        return Ok();
    }
    
    [HttpGet]
    [Route("all")]
    public async Task<IActionResult> GetAllInfrastructures()
    {
        var infraList = await _infrastructureService.GetAllInfrastrcturesAsync();
        var infraDtoList = infraList.Select(i => i.ToInfrastructureDto());
        
        // don't return List<> here (which is lazy-evaluated) cause we will get strange error about missing DbContext - misleading as fcuk
        // just return IEnumerable
        return Ok(infraDtoList);
    }

    [HttpGet]
    [Route("{name}")]
    public async Task<IActionResult> GetInfrastructureByName(string name)
    {
        var infra = await _infrastructureService.GetInfrastructureByNameAsync(name);
        var infraDto = infra.ToInfrastructureDto();
        return Ok(infraDto);
    }
    
    
    [HttpDelete("name:string")]
    public async Task<IActionResult> DeleteInfrastructure(string infrastructureName)
    {
        var deleted = await _infrastructureService.DeleteAsync(infrastructureName);
        if (deleted)
        {
            return Ok("Infrastructure deleted: " + infrastructureName);
        }
        return BadRequest("Infrastructure not found");
    }
}