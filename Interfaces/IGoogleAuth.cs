using Google.Apis.Auth;

namespace ProvEditorNET.Interfaces;

public interface IGoogleAuth
{
    Task<GoogleJsonWebSignature.Payload> AuthenticateIdToken(string token);
}