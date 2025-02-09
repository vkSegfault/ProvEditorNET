using System.Security.Claims;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using ProvEditorNET.Interfaces;

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
        
        JObject requestBodyJson = JObject.Parse( requestBody.ToString() );
        string idToken = requestBodyJson["idToken"]?.ToString();
        string accessToken = requestBodyJson["accessToken"]?["access_token"]?.ToString();

        // handle idToken
        if (idToken is not null)
        {
            GoogleJsonWebSignature.Payload payload = await _identityService.AuthenticateGoogleUserIdTokenAsync(idToken);
            if (payload is null )
            {
                return BadRequest("Invalid token");
            }
            else
            {
                bool exists = await _identityService.UserExistsAsync(payload.Email);

                if (exists)
                {
                    Console.WriteLine($"User {payload.Email} already exists - login in");
                    var claimsPrincipal = await _identityService.GenerateAccessToken(payload.Email );
                    if (claimsPrincipal is null)
                    {
                        Console.WriteLine("User exists but administrator didn't approve him/her");
                        return Unauthorized("User exists but administrator didn't approve him/her");
                    }
                    Console.WriteLine($"Bearer access token: {claimsPrincipal}");
                    // await _identityService.SignInAsync( payload.Email, claimsPrincipal );   // manually SignIn user because External Google SSO is not managed by COre Identity service
                    Console.WriteLine("User logged in CONTROLLER BASE: " + User.Claims.FirstOrDefault());   // this lines returns ID of logged by Google SSO user
                    return SignIn(claimsPrincipal);
                }
                else
                {
                    Console.WriteLine("User doesn't exist - attempt to register new one");
                    await _identityService.RegisterUserAsync( payload.Email, "" );
                    await _identityService.SendConfirmationEmailAsync( payload.Email );
                    return Created("Email with user activation link has been sent to administrator", null);
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

    // this endpoint is generally to be used only by confirmation mail we sent from calling /google endpoint - there is no other way to get confirmation token
    [HttpGet]
    [ActionName("confirmemailcustom")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
    {
        Console.WriteLine("Email: " + email + " Token: " + token);
        var verified = await _identityService.VerifyEmailAsync(email, token);
        if (verified)
        {
            Console.WriteLine($"Email {email} verified");
            var claimsPrincipal = await _identityService.GenerateAccessToken(email);
            Console.WriteLine($"Bearer access token: {claimsPrincipal}");

            // return RedirectToAction("accesstoken", new { email, token });
            return Ok($"Email {email} verified - you can close this page");
            
            // what should happend here ideally:
            // 1. we return: Ok($"Email {email} verified")
            // 2. we call some Redirect (code 302) and actually 
            
            return SignIn(claimsPrincipal);
        }
        else
        {
            return BadRequest("Email not verified");
        }
    }
    
    [HttpGet]
    [ActionName("accesstoken")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAccessToken([FromQuery] string email)
    {
        var claimsPrincipal = await _identityService.GenerateAccessToken(email);
        if (claimsPrincipal != null)
        {
            Console.WriteLine($"Access token: {claimsPrincipal}");  // TODO - get bearer access token from this claimsPrincipal
            return SignIn(claimsPrincipal);
        }
        else
        {
            return NotFound($"User {email} not found or has been not verified by administrator");
        }
    }
    
    [HttpPost]
    [ActionName("logout")]
    // [Authorize]
    [Authorize(Roles = "Admin")]
    [Authorize(Policy = "AdminPolicy")]
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