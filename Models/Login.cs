using Microsoft.AspNetCore.Identity;

namespace ProvEditorNET.Models;

public class Login : IdentityUser
{
    public string? FirstName { get; set; } = string.Empty();
    public string? LastName { get; set; } = string.Empty();
}