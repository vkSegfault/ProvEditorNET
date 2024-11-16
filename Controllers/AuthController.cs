using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using ProvEditorNET.Interfaces;

namespace ProvEditorNET.Controllers;

// idToken sent from client (frontend) let us AUTHENTICATE user
// accessToken sent from client (frontend) let us AUTHORIZE user
// ************
// once we get idToken we just confirms who he is (and if the idToken is correct)
// 

[ApiController]
[Route("api/v1/[controller]/[action]")]
public class AuthController : ControllerBase
{
    private readonly IGoogleAuth _googleAuth;

    public AuthController(IGoogleAuth googleAuth)
    {
        _googleAuth = googleAuth;
    }
    
    
    [HttpPost(Name = "Authorize User using Google SSO")]
    [ActionName("google")]
    [AllowAnonymous]
    public async Task<ActionResult> AuthorizeGoogleSso([FromBody] object requestBody)
    {
        Console.WriteLine(requestBody);
        if (requestBody is null)
        {
            return BadRequest("Request body is null");
        }
        else
        {
            JObject requestBodyJson = JObject.Parse( requestBody.ToString() );
            string idToken = requestBodyJson["idToken"]?.ToString();

            if (idToken is not null)
            {
                Console.WriteLine($"Validating Google Access token: {idToken}");
                GoogleJsonWebSignature.Payload payload = await _googleAuth.AuthenticateIdToken(idToken);
                if (payload is null )
                {
                    return BadRequest("Invalid token");
                }
                else
                {
                    Console.WriteLine("Payload: " + payload.Name);
                    return Ok(payload);
                }
            }
            else
            {
                return BadRequest("No access_token in request body");
            }
        }

        // TODO - remove thread sleep - just used for testing frontend delays
        Thread.Sleep(2000);
        return Ok();
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