using System.Security.Claims;
using System.Text;
using System.Web;
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
    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _configuration;

    public IdentityService(IdentityDbContext identityDbContext, UserManager<IdentityUser> userManager, IEmailSender emailSender, IConfiguration configuration)
    {
        _identityRepository = identityDbContext;
        _userManager = userManager;
        _emailSender = emailSender;
        _configuration = configuration;
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

        string link = $"{_configuration.GetValue<string>("hostDev")}/api/v1/auth/confirmemail?email={email}&token={confirmEmailTokenEncoded}";
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
        var mailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
        if (mailConfirmed == true)
        {
            var claimsPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[] { new Claim(ClaimTypes.Email, email)},
                    BearerTokenDefaults.AuthenticationScheme
                )
            );
            return claimsPrincipal;
        }
        
        return null;
    }
}