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
}