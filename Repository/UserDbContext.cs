using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ProvEditorNET.Repository;

public class UserDbContext: IdentityDbContext
{
    public UserDbContext(DbContextOptions options) : base(options)
    {
        
    }
}