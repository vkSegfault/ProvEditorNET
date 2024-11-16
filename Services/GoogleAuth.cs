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
            return payload;
        }
        catch (Exception e)
        {
            Console.WriteLine("Invalid Google Token: " + e);
        }

        return null;
    }
}