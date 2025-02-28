using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProvEditorNET.DTO;
using ProvEditorNET.Interfaces;

namespace ProvEditorNET.Controllers;

[ApiController]
// [Route("api/v1/auth/[controller]/[action]")]
[Route("api/v1/auth/[controller]")]
public class RolesController : ControllerBase
{
    IIdentityService _identityService;
    
    public RolesController(IIdentityService identityService)
    {
        _identityService = identityService;
    }
    
    [HttpGet]
    // [ActionName("GetAll")]
    // [Authorize(Policy = "AdminPolicy")]
    [Authorize(Roles = "Admin")]
    public ActionResult<IEnumerable<string>> GetAllRoles()
    {
        var roles =  _identityService.GetAllRoles();
        return Ok(roles);
    }
    
    [HttpGet("{email}")]   // we dont use {email:string} because string is default route type constraint
    // [ActionName("GetUserRoles")]
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
    // [ActionName("Add")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> AddRole([FromBody] RoleDto roleDto)
    {
        await _identityService.CreateRoleAsync(roleDto.RoleName);
        return Created(roleDto.RoleName, roleDto);
    }
    
    [HttpDelete]
    // [ActionName("Delete")]
    public async Task<ActionResult> DeleteRole([FromBody] RoleDto roleDto)
    {
        bool deleted = await _identityService.DeleteRoleAsync(roleDto.RoleName);
        return deleted ? NoContent() : BadRequest("Role not found");
    }

    [HttpPost("user")]
    // [ActionName("AddUserToRole")]
    // [AllowAnonymous]
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
    
    
    [HttpDelete("user")]
    // [ActionName("RemoveUserFromRole")]
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
    
    // premise of this endpoint is to give admin rights to first registered user
    // call it just after first registration
    // TODO
    // call this functionallity upon user registration - we check if registered user is first one in DB, if it is then give him admin rights
    [HttpGet("give-admin-roles")]
    // [ActionName("giveadminrights")]
    [AllowAnonymous]
    public async Task<IActionResult> GiveAdminRights()
    {
        // check if there are no roles - if not create default ones: admin, user, observer
        var roles =  _identityService.GetAllRoles();
        if ( roles.Count() == 0 )
        {
            await _identityService.CreateRoleAsync("Admin");
            await _identityService.CreateRoleAsync("User");
            await _identityService.CreateRoleAsync("Observer");
        }

        // check if there is only 1 user - first one will get all default rights
        var users = await _identityService.GetAllUsers();
        if ( users.Count() == 0 )
        {
            return BadRequest("There are no user - please register first one");
        }
        else if (users.Count() > 1)
        {
            return BadRequest("There are more than 1 users - first one already has admin rigths");
        }
        else if ( users.Count() == 1 )
        {
            await _identityService.AddUserToRoleAsync(users.First().Email, "Admin");
            await _identityService.AddUserToRoleAsync(users.First().Email, "User");
            await _identityService.AddUserToRoleAsync(users.First().Email, "Observer");
        }

        return Ok("First user was given Admin, User and Observer rights");
    }
}