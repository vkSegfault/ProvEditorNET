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
public class InfrastructuresController : ControllerBase
{
    private readonly IInfrastructureService _infrastructureService;
    private readonly IMeterFactory _meterFactory;
    private readonly IRedisService _redisService;

    public InfrastructuresController(IInfrastructureService infrastructureService, IMeterFactory meterFactory, IRedisService redisService)
    {
        _infrastructureService = infrastructureService;
        _meterFactory = meterFactory;
        _redisService = redisService;
    }
    
    [HttpPost]
    public async Task<ActionResult<Infrastructure>> CreateInfrastructure([FromBody] InfrastructureDto infrastructureDto)
    {
        var infra = infrastructureDto.ToInfrastructure();
        await _infrastructureService.CreateAsync(infra);
        
        var meter = _meterFactory.Create("InfrastructureAddedMeter");
        var instrument = meter.CreateCounter<int>("country_added_counter");
        instrument.Add(1);   // add 1 to this meter everytime we add Country

        return Created( $"api/v1/infrastrctures/{infrastructureDto.InfrastructureName}", infrastructureDto);
    }
    
    [HttpGet]
    // [Route("all")]
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
    
    [HttpPut]
    [Route("{name}")]
    public async Task<IActionResult> UpdateInfrastructure([FromRoute] string name, [FromBody] InfrastructureDto infrastructureDto)
    {
        var infrastructure = await _infrastructureService.GetInfrastructureByNameAsync(name);

        if (infrastructure is not null)
        {
            infrastructure.Name = infrastructureDto.InfrastructureName;
            infrastructure.Factor = infrastructureDto.Factor;
            infrastructure.Price = infrastructureDto.BuildPrice;
            infrastructure.MothlyCost = infrastructureDto.MonthlyCost;
            infrastructure.Technology = infrastructureDto.TechnologyLevelRequired;
            infrastructure.Notes = infrastructureDto.Notes;
            await _infrastructureService.SaveChangesAsync();
            
            await _redisService.InvalidateCacheAsync("infrastructure_all");
            await _redisService.InvalidateCacheAsync( $"infrastructure:{name}" );
            
            return Ok($"Infrastructure {name} updated");
        }
        else
        {
            return NotFound("Infrastructure not found");
        }
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