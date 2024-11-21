namespace ProvEditorNET.Controllers;

// ******************************
// We can authorize user both with Roles and policies (per whole Controller or per Method)
// Role: [Authorize(Roles = "Admin")]
// Policy: [Authorize(Policy = "AdminPolicy")]

[ApiController]
[Route("api/v1/[controller]/[action]")]
[Authorize(Policy = "AdminPolicy")]
public class RoleController : ControllerBase
{
    // GET all existing roles
    // POST - add role to list of roles
    // DELETE role from the list of users
    // TODO - add role to user
    // TODO - remove role from user
    // TODO - 
}