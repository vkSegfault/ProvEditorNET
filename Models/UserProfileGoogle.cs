namespace ProvEditorNET.Models;

public class UserProfileGoogle
{
    public string Email { get; set; } = String.Empty;
    public string Password { get; } = String.Empty;   // password for Google SSO login should always be empty
}