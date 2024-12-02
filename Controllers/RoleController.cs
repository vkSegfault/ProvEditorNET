using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProvEditorNET.DTO;
using ProvEditorNET.Interfaces;

namespace ProvEditorNET.Controllers;

[ApiController]
[Route("api/v1/auth/[controller]/[action]")]
public class RoleController : ControllerBase
{
    IIdentityService _identityService;
    
    public RoleController(IIdentityService identityService)
    {
        _identityService = identityService;
    }
    
    [HttpGet]
    [ActionName("GetAll")]
    // [Authorize(Policy = "AdminPolicy")]
    [Authorize(Roles = "Admin")]
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

    [HttpGet("{email}")]   // we dont use {email:string} because string is default route type constraint
    [ActionName("GetUserRoles")]
    [AllowAnonymous] // TODO - remove allowAnonymous
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetUserRoles([FromRoute] string email)
    {
        if (email is not null)
        {
            var roles = await _identityService.GetUserRolesAsync(email);
            return Ok(roles);
        }
        else
        {
            return BadRequest("Email in path is required");
        }
    }

    [HttpPost]
    [ActionName("AddUserToRole")]
    [AllowAnonymous] // TODO - remove allowAnonymous
    public async Task<ActionResult> AddUserToRole([FromQuery] string email, [FromQuery] string roleName)
    {
        if (email is null)
        {
            return BadRequest("Email query string is required");
        }
        if (roleName is null)
        {
            return BadRequest("Role name query string is required");
        } 
        var added = await _identityService.AddUserToRoleAsync(email, roleName);
        return added ? NoContent() : BadRequest("Role not added");
    }
    
    
    [HttpDelete]
    [ActionName("RemoveUserFromRole")]
    [AllowAnonymous] // TODO - remove allowAnonymous
    public async Task<ActionResult> RemoveUserFromRole([FromQuery] string email, [FromQuery] string roleName)
    {
        if (email is null)
        {
            return BadRequest("Email query string is required");
        }
        if (roleName is null)
        {
            return BadRequest("Role name query string is required");
        }
        
        var removed = await _identityService.RemoveUserFromRoleAsync(email, roleName);
        return removed ? NoContent() : BadRequest("Role not added");
    }
}