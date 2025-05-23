using Microsoft.AspNetCore.Identity;

namespace ProvEditorNET.Models;

public class User : IdentityUser
{
    public string? FirstName { get; set; } = String.Empty;
    public string? LastName { get; set; } = String.Empty;
}