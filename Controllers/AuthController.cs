using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ProvEditorNET.Controllers;

[ApiController]
[Route("api/v1/[controller]/[action]")]
public class AuthController : ControllerBase
{
    [HttpPost(Name = "Authorize User using Google SSO")]
    [ActionName("google")]
    [Authorize]
    public async Task<ActionResult<List<String>>> AuthorizeGoogleSSO()
    {
        var provinces = new List<String>{ "pomerania", "masovia" };
        provinces.Add("silesia");

        // TODO - remove thread sleep - just used for testing frontend delays
        Thread.Sleep(2000);
        return Ok(provinces);
    }

    [HttpPost]
    [ActionName("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(SignInManager<IdentityUser> signInManager, [FromBody] object obj)
    {
        // if Frontend calls POST on this endpoint with empty body we assume it means we should logout 
        if (obj != null)
        {
            await signInManager.SignOutAsync();
            return Ok();
        }

        return Unauthorized();
    }
}