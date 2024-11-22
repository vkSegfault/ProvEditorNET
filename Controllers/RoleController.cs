using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProvEditorNET.DTO;
using ProvEditorNET.Interfaces;

namespace ProvEditorNET.Controllers;

[ApiController]
[Route("api/v1/[controller]/[action]")]
[Authorize(Policy = "AdminPolicy")]
public class RoleController : ControllerBase
{
    IIdentityService _identityService;
    
    public RoleController(IIdentityService identityService)
    {
        _identityService = identityService;
    }
    
    [HttpGet]
    [ActionName("GetAll")]
    [AllowAnonymous]   // TODO - remove allowAnonymous
    public ActionResult<IEnumerable<string>> GetAllRoles()
    {
        var roles =  _identityService.GetAllRoles();
        return Ok(roles);
    }
    
    [HttpPost]
    [ActionName("Add")]
    [AllowAnonymous] // TODO - remove allowAnonymous
    public async Task<ActionResult> AddRole([FromBody] RoleDto roleDto)
    {
        await _identityService.CreateRoleAsync(roleDto.RoleName);
        return Created(roleDto.RoleName, roleDto);
    }
    
    // DELETE role from the list 
    [HttpDelete]
    [ActionName("Delete")]
    [AllowAnonymous] // TODO - remove allowAnonymous
    public async Task<ActionResult> DeleteRole([FromBody] RoleDto roleDto)
    {
        bool deleted = await _identityService.DeleteRoleAsync(roleDto.RoleName);
        return deleted ? NoContent() : BadRequest("Role not found");
    }
    
    // TODO - add role to user
    // TODO - remove role from user
}