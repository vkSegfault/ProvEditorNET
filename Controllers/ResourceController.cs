using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProvEditorNET.DTO;
using ProvEditorNET.Helpers;
using ProvEditorNET.Interfaces;
using ProvEditorNET.Mappers;
using ProvEditorNET.Models;
using ProvEditorNET.Services;

namespace ProvEditorNET.Controllers;

[ApiController]
[Route($"{ApiEndpoints.ApiBase}/[controller]")]
[Authorize(Roles = "Admin,User")]   // Admin OR User - not necessarily both
public class ResourcesController : ControllerBase
{
    private readonly IResourceService _resourceService;
    private readonly IRedisService _redisService;

    public ResourcesController(IResourceService resourceService, RedisService redisService)
    {
        _resourceService = resourceService;
        _redisService = redisService;
    }
    
    [HttpPost]
    public async Task<ActionResult<Resource>> CreateResource([FromBody] ResourceDto resourceDto)
    {
        var resource = resourceDto.ToResource();
        await _resourceService.CreateAsync(resource);

        return Created( $"api/v1/resources/{resourceDto.ResourceName}", resourceDto);
    }
    
    [HttpGet]
    // [Route("all")]
    public async Task<IActionResult> GetAllResources()
    {
        var resourceList = await _resourceService.GetAllResourcesAsync();
        var resourceDtoList = resourceList.Select(i => i.ToResourceDto());

        // don't return List<> here (which is lazy-evaluated) cause we will get strange error about missing DbContext - misleading as fcuk
        // just return IEnumerable
        return Ok(resourceDtoList);
    }

    [HttpGet]
    [Route("{name}")]
    public async Task<IActionResult> GetResourceByName(string name)
    {
        var resource = await _resourceService.GetResourceByNameAsync(name);
        var resourceDto = resource.ToResourceDto();
        return Ok(resourceDto);
    }
    
    [HttpPut]
    [Route("{name}")]
    public async Task<IActionResult> UpdateResource([FromRoute] string name, [FromBody] ResourceDto resourceDto)
    {
        var resource = await _resourceService.GetResourceByNameAsync(name);

        if (resource is not null)
        {
            resource.Name = resourceDto.ResourceName;
            resource.Notes = resourceDto.Notes;
            await _resourceService.SaveChangesAsync();
            
            await _redisService.InvalidateCacheAsync("resource_all");
            await _redisService.InvalidateCacheAsync( $"resource:{name}" );
            
            return Ok($"Resource {name} updated");
        }
        else
        {
            return NotFound("Resource not found");
        }
    }
    
    
    [HttpDelete("name:string")]
    public async Task<IActionResult> DeleteResource(string resourceName)
    {
        var deleted = await _resourceService.DeleteResourceAsync(resourceName);
        if (deleted)
        {
            return Ok("Resource deleted: " + resourceName);
        }
        return BadRequest("Resource not found");
    }
}