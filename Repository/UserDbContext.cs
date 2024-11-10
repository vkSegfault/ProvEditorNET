using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ProvEditorNET.Repository;

// public class UserDbContext: IdentityDbContext<IdentityUser>
public class UserDbContext: IdentityDbContext<IdentityUser>
{
    // public UserDbContext(DbContextOptions options) : base(options)
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
        
    }
}