using System.Runtime.InteropServices.Marshalling;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProvEditorNET.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProvinceController: ControllerBase
{
    [HttpGet("healthcheck", Name = "Healthcheck")]
    public async Task<ActionResult<Boolean>> Healthcheck()
    {
        return Ok(true);
    }
    
    // this endpoint is secured
    // (1 --------------- Bearer Access Token) go to /login endpoint and pass your login and password:
    // curl -X 'POST' 'http://localhost:5077/login' -H 'accept: application/json' -H 'Content-Type: application/json' -d '{
    //   "email": "yourmail@mail.com",
    //   "password": "YourPassword",
    //   "twoFactorCode": "string",
    //   "twoFactorRecoveryCode": "string"
    // }'
    // we will get access token upon succesful login, copy it
    // to any secured endpoint just add this token like so: -H 'accept: text/plain' -H 'Authorization: Bearer OMx_36NgZOqRSICWKSLRGGF'
    // (2 ------------- Cookies - instead copying token manually it's saved as cookie in browser cache and we don't need to pass it anywhere), go to /login and pass your creds:
    // curl -X 'POST' 'http://localhost:5077/login?useCookies=true'' -H 'accept: application/json' -H 'Content-Type: application/json' -d '{
    //   "email": "yourmail@mail.com",
    //   "password": "YourPassword",
    //   "twoFactorCode": "string",
    //   "twoFactorRecoveryCode": "string"
    // }'
    [HttpGet(Name = "GetProvinces"), Authorize]
    public async Task<ActionResult<List<String>>> GetAllProvinces()
    {
        var provinces = new List<String>{ "pomerania", "masovia" };
        provinces.Add("silesia");

        return Ok(provinces);
    }
}