using Microsoft.AspNetCore.Identity;

namespace ProvEditorNET.Interfaces;

public interface IIdentityService
{
    Task<bool> UserExistsAsync(string email);
}