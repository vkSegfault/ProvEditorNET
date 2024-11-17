using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProvEditorNET.Interfaces;
using ProvEditorNET.Repository;

namespace ProvEditorNET.Services;

public class IdentityService : IIdentityService
{
    private readonly IdentityDbContext _identityRepository;

    public IdentityService(IdentityDbContext identityDbContext)
    {
        _identityRepository = identityDbContext;
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
    }
}