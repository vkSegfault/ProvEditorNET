using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProvEditorNET.Models;

namespace ProvEditorNET.Repository;

// IdentityUser is .NET Core predefined user, but we can add our custom one (with additional fields) by inheriting from IdentityUser and provide it here
public class IdentityDbContext: IdentityDbContext<IdentityUser>
// public class IdentityDbContext: IdentityDbContext<User>
{
    // public UserDbContext(DbContextOptions options) : base(options)
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
        
    }
}