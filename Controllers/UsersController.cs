using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProvEditorNET.Helpers;
using ProvEditorNET.Interfaces;

namespace ProvEditorNET.Controllers;

[ApiController]
[Route($"{ApiEndpoints.ApiBase}/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IIdentityService _identityService;
    
    public UsersController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpGet("users")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUsers()
    {
        IEnumerable<IdentityUser> users = await _identityService.GetAllUsers();
        return Ok(users);
    }
}