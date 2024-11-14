using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProvEditorNET.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost(Name = "Authorize User using Google SSO")]
    [Authorize]
    public async Task<ActionResult<List<String>>> AuthorizeGoogleSSO()
    {
        var provinces = new List<String>{ "pomerania", "masovia" };
        provinces.Add("silesia");

        // TODO - remove thread sleep - just used for testing frontend delays
        Thread.Sleep(2000);
        return Ok(provinces);
    }
}