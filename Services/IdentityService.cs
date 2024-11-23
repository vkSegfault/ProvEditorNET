using System.Security.Claims;
using System.Text;
using System.Web;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using ProvEditorNET.Interfaces;
using ProvEditorNET.Repository;

namespace ProvEditorNET.Services;

public class IdentityService : IIdentityService
{
    private readonly IdentityDbContext _identityRepository;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _configuration;
    private readonly IGoogleAuth _googleAuth;

    public IdentityService(IdentityDbContext identityDbContext, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IEmailSender emailSender, IConfiguration configuration, IGoogleAuth googleAuth)
    {
        _identityRepository = identityDbContext;
        _userManager = userManager;
        _roleManager = roleManager;
        _emailSender = emailSender;
        _configuration = configuration;
        _googleAuth = googleAuth;
    }
    
    public async Task<bool> UserExistsAsync(string email)
    {
        var user = await _identityRepository.Users.SingleOrDefaultAsync(u => u.Email == email);
        Console.WriteLine(user);
        if (user is not null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task RegisterUserAsync(string email, string password)
    {
        // we cannot call /register endpoint directly because we set globally it needs password and for SSO we don't use password - we authenticate with idToken instead
        // we add user to DB instead
        IdentityUser user = new IdentityUser();
        user.UserName = email;
        user.NormalizedUserName = email.ToUpper();
        user.Email = email;
        user.NormalizedEmail = email.ToUpper();
        user.LockoutEnabled = true;
        
        await _identityRepository.Users.AddAsync(user);
        await _identityRepository.SaveChangesAsync();
        
        // TODO
        // send verification email - call EmailSender service manually to do so
        Console.WriteLine("User created");
    }

    public async Task<string> SendConfirmationEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email.ToUpper());
        Console.WriteLine("Found user: " + user.Email);
        var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmEmailTokenEncoded = HttpUtility.UrlEncode(confirmEmailToken);
        Console.WriteLine("Confirmation token: " + confirmEmailToken);

        string link = $"{_configuration.GetValue<string>("Host:Dev")}/api/v1/auth/confirmemail?email={email}&token={confirmEmailTokenEncoded}";
        Console.WriteLine("Confirmation link: " + link);
        string mailContent = $"<a href='{link}'>Verify email</a>";
        await _emailSender.SendEmailAsync("adtofaust@gmail.com", $"New User Registration: {email}", mailContent);
        
        Console.WriteLine("Confirmation email sent");
        
        return confirmEmailToken;
    }

    public async Task<bool> VerifyEmailAsync(string email, string token)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user != null)
        {
            var valid = await _userManager.ConfirmEmailAsync(user, token);
            if (valid == IdentityResult.Success)
            {
                Console.WriteLine(valid);
                Console.WriteLine($"Email {email} verified");
                return true;
                
            }
            else
            {
                Console.WriteLine("Email not confirmed - invalid token");
                return false;
            }
        }
        else
        {
            Console.WriteLine("Email not confirmed - user not found");
            return false;
        }
    }

    public async Task<ClaimsPrincipal> GenerateAccessToken(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        var mailConfirmed = user != null ? await _userManager.IsEmailConfirmedAsync(user) : false;
        if (mailConfirmed == true)
        {
            List<Claim> allClaims = await GetUserClaims(email);
            allClaims.Append( new Claim(ClaimTypes.Email, email) );
            
            ClaimsIdentity claimsIdentity = new ClaimsIdentity( allClaims, BearerTokenDefaults.AuthenticationScheme );
            
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal( claimsIdentity );
            
            // var claimsPrincipal = new ClaimsPrincipal(
            //     new ClaimsIdentity(
            //         new[] { new Claim(ClaimTypes.Email, email), claims },
            //         BearerTokenDefaults.AuthenticationScheme
            //     )
            // );
            return claimsPrincipal;
        }
        
        return null;
    }

    public async Task<List<Claim>> GetUserClaims(string email)
    {
        if (await UserExistsAsync(email))
        {
            var roles = await GetUserRolesAsync(email);
            var claims = new List<Claim>();
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            return claims;
        }
        return new List<Claim>();
    }
    
    public async Task<GoogleJsonWebSignature.Payload> AuthenticateGoogleUserIdTokenAsync(string idToken)
    {
        Console.WriteLine($"Validating Google Access token: {idToken}");
        GoogleJsonWebSignature.Payload payload = await _googleAuth.AuthenticateIdToken(idToken);
        
        if (payload is not null)
        {
            if (payload.Audience.Equals(_configuration.GetValue<string>("Google:client_id")))
            {
                return payload;
                
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }
    
    // NOTE that we add users to Roles, not other way around
    public IQueryable<IdentityRole> GetAllRoles()
    {
        IQueryable<IdentityRole> roles = _roleManager.Roles;
        foreach (IdentityRole role in roles)
        {
            Console.WriteLine(role.ToString());
        }
        return roles;
    } 
    public async Task CreateRoleAsync(string roleName)
    {
        var role = new IdentityRole(roleName);
        await _roleManager.CreateAsync(role);
    }

    public async Task<bool> DeleteRoleAsync(string roleName)
    {
        bool roleExists = await _roleManager.RoleExistsAsync(roleName);
        if (roleExists == true)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            await _roleManager.DeleteAsync(role!);
            Console.WriteLine("Role deleted");
            return true;
        }
        {
            Console.WriteLine($"Role {roleName} doesn't exists");
            return false;
        }
    }
    
    public async Task<List<string>> GetUserRolesAsync(string email)
    {
        if (await UserExistsAsync(email))
        {
            var user = await _userManager.FindByEmailAsync(email);
            var userRoles = await _userManager.GetRolesAsync(user);
            return userRoles.ToList();
        }
        
        var authClaims = new List<Claim>
        {
            // new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
            // new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        return new List<string>();
    }
    public async Task<bool> AddUserToRoleAsync(string email, string roleName)
    {
        if (await UserExistsAsync(email))
        {
            var user = await _userManager.FindByEmailAsync(email);
            await _userManager.AddToRoleAsync(user!, roleName);
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<bool> RemoveUserFromRoleAsync(string email, string roleName)
    {
        if (await UserExistsAsync(email))
        {
            var user = await _userManager.FindByEmailAsync(email);
            await _userManager.RemoveFromRoleAsync(user!, roleName);
            return true;
        }

        return false;
    }
}