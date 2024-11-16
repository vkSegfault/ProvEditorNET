using Google.Apis.Auth;
using ProvEditorNET.Interfaces;

namespace ProvEditorNET.Services;


public class GoogleAuth : IGoogleAuth
{
    public async Task<GoogleJsonWebSignature.Payload> AuthenticateIdToken(string token)
    {
        try
        {
            GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(token);
            if ( payload != null || string.IsNullOrEmpty(payload.Email) )
            {
                return payload;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Invalid Google Token: " + e);
        }

        return null;
    }

    public async Task<string> AuthorizeAccessToken(string token)
    {
        Console.WriteLine(token);
        using var httpClient = new HttpClient();
        var res = httpClient.GetAsync($"https://www.googleapis.com//oauth2/v3/userinfo?{token}", HttpCompletionOption.ResponseContentRead);

        return String.Empty;
    }
}