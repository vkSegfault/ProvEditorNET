using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using ProvEditorNET.Interfaces;
using ProvEditorNET.Models;

namespace ProvEditorNET.Controllers;


[ApiController]
[Route("api/v1/[controller]/[action]")]
public class AuthController : ControllerBase
{
    private readonly IGoogleAuth _googleAuth;
    private readonly IIdentityService _identityService;

    public AuthController(IGoogleAuth googleAuth, IIdentityService identityService)
    {
        _googleAuth = googleAuth;
        _identityService = identityService;
    }
    
    
    [HttpPost(Name = "Authorize User using Google SSO")]
    [ActionName("google")]
    [AllowAnonymous]
    public async Task<ActionResult> AuthorizeGoogleSso([FromBody] object requestBody)
    {
        // TODO
        // new userProfileGoogle - create new model class
        // TODO
        // var userProfileGoole = getUserData( idToken || accessToken )
        // _identityDBRepository.userExsits?( userProfileGoole.email )
        // if exists call login endpoint and send new Bearer token to frontend
        // if not call /register endpoint

        
        Console.WriteLine(requestBody);
        if (requestBody is null)
        {
            return BadRequest("Request body is null");
        }

        UserProfileGoogle user;
        
        JObject requestBodyJson = JObject.Parse( requestBody.ToString() );
        string idToken = requestBodyJson["idToken"]?.ToString();
        string accessToken = requestBodyJson["accessToken"]?["access_token"]?.ToString();

        // handle idToken
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
                bool exists = await _identityService.UserExistsAsync(payload.Email);

                if (exists)
                {
                    Console.WriteLine("User already exists - login in");
                    // login him/generate bearer token (how to generate one without calling /login or how to login without password?)
                }
                else
                {
                    Console.WriteLine("User doesn't exist - signing in");
                    // call register (but how to register without password?)
                }
                Console.WriteLine("Payload.Name: " + payload.Name);
                return Ok(payload);
            }
        }
        
        // handle accessToken
        if (accessToken is not null)
        {
            await _googleAuth.AuthorizeAccessToken(accessToken);
        }
        else
        {
            return BadRequest("No access_token in request body");
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