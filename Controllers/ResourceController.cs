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
public class ResourcesController : ControllerBase
{
    private readonly IResourceService _resourceService;

    public ResourcesController(IResourceService resourceService)
    {
        _resourceService = resourceService;
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