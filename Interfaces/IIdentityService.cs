using Microsoft.AspNetCore.Identity;

namespace ProvEditorNET.Interfaces;

public interface IIdentityService
{
    Task<bool> UserExistsAsync(string email);
    Task RegisterUserAsync(string email, string password);   // for SSO password should be empty
    Task SendConfirmationEmailAsync(string email);
}