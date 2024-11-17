using Google.Apis.Auth;
using ProvEditorNET.Interfaces;

namespace ProvEditorNET.Services;


public class GoogleAuth : IGoogleAuth
{
    public async Task<GoogleJsonWebSignature.Payload> AuthenticateIdToken(string token)
    {
        Console.WriteLine("Authenticating ID token: ");
        Console.WriteLine(token);
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
        Console.WriteLine("Authorizing access token: ");
        Console.WriteLine(token);
        using var httpClient = new HttpClient();
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://www.googleapis.com/oauth2/v3/userinfo");
        request.Headers.Add("Authorization", "Bearer " + token);
        Console.WriteLine(request.ToString());
        var res = await httpClient.SendAsync( request );
        res.EnsureSuccessStatusCode();
        var jsonRes = await res.Content.ReadAsStringAsync();
        Console.WriteLine("Response calling www.googleapis.com/oauth2/v3/userinfo: " + jsonRes);

        return jsonRes;
    }
}