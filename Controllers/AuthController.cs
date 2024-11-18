using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using ProvEditorNET.Interfaces;
using ProvEditorNET.Models;
using ProvEditorNET.Services;

namespace ProvEditorNET.Controllers;


[ApiController]
[Route("api/v1/[controller]/[action]")]
public class AuthController : ControllerBase
{
    private readonly IGoogleAuth _googleAuth;
    private readonly IIdentityService _identityService;
    private readonly IEmailSender _emailSender;

    public AuthController(IGoogleAuth googleAuth, IIdentityService identityService, IEmailSender emailSender)
    {
        _googleAuth = googleAuth;
        _identityService = identityService;
        _emailSender = emailSender;
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
                // bool exists = await _identityService.UserExistsAsync("gonzo@gonzo.com");

                if (exists)
                {
                    Console.WriteLine("User already exists - login in");
                    // login him/generate bearer token (how to generate one without calling /login or how to login without password?)
                }
                else
                {
                    Console.WriteLine("User doesn't exist - attempt to register new one");
                    await _identityService.RegisterUserAsync( payload.Email, "" );
                    // await _identityService.RegisterUserAsync( "gonzo@gonzo.com", "" );
                    // await _emailSender.SendEmailAsync("adtofaust@gmail.com", "Approval for New User Registration", "New user registration - should you choose to accept?");   //send activation link
                    // calling /resendConfirmationEmail with body: { "Email": "user email" } should
                    await _identityService.SendConfirmationEmailAsync( payload.Email );
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

    [HttpGet]
    [ActionName("confirm")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
    {
        Console.WriteLine("Email: " + email + " Token: " + token);
        var verified = await _identityService.VerifyEmailAsync(email, token);
        if (verified)
        {
            // generate Bearer authorization token and send it back to client
            return Ok("Email verified");
        }
        else
        {
            return BadRequest("Email not verified");
        }
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