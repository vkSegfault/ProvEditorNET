using System.Security.Claims;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;

namespace ProvEditorNET.Interfaces;

public interface IIdentityService
{
    Task<IEnumerable<IdentityUser>> GetAllUsers();
    Task<bool> UserExistsAsync(string email);
    string GetLoggedInUsername();
    Task RegisterUserAsync(string email, string password);   // for SSO password should be empty
    Task<string> SendConfirmationEmailAsync(string email);
    Task<bool> VerifyEmailAsync(string email, string token);
    Task<ClaimsPrincipal> GenerateAccessToken(string email);
    Task<List<Claim>> GetUserClaims(string email);
    Task<GoogleJsonWebSignature.Payload> AuthenticateGoogleUserIdTokenAsync(string idToken);
    IQueryable<IdentityRole> GetAllRoles();
    Task CreateRoleAsync(string roleName);
    Task<bool> DeleteRoleAsync(string roleName);
    Task<List<string>> GetUserRolesAsync(string email);
    Task<bool> AddUserToRoleAsync(string email, string roleName);
    Task<bool> RemoveUserFromRoleAsync(string email, string roleName);
}