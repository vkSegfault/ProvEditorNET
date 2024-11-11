using Microsoft.AspNetCore.Identity;

namespace ProvEditorNET.Models;

public class User : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}