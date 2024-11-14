using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ProvEditorNET.Models;
using ProvEditorNET.Repository;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddDbContext<IdentityDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;

    options.User.RequireUniqueEmail = true;
});

// use simple authorization directly from Indentity package or cutomized authenitaction (needed for SSO)
builder.Services.AddAuthorization();
builder.Services
    .AddIdentityApiEndpoints<IdentityUser>()   // this add /register, /login etc endpoints
    .AddEntityFrameworkStores<IdentityDbContext>();   // this tells to use our DB for storing identity users

// alternative when using our custom defined IdentityUser
// builder.Services.AddAuthentication(options =>
//     {
//         options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//         options.DefaultChallengeScheme = BearerTokenDefaults.AuthenticationScheme;
//     })
//     .AddCookie(IdentityConstants.ApplicationScheme)
//     .AddBearerToken(IdentityConstants.BearerScheme);
// builder.Services.AddIdentityCore<User>().AddEntityFrameworkStores<IdentityDbContext>().AddApiEndpoints();

// Google SSO
// builder.Services.AddAuthentication().AddGoogle(googleOptions =>
// {
//     googleOptions.SignInScheme = IdentityConstants.ExternalScheme;
//     googleOptions.ClientId = "592195124025-6g06a3tddd3fpu494rsrplopn83f7jb2.apps.googleusercontent.com";
//     googleOptions.ClientSecret = "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";
// });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigin", policyBuilder => policyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
    options.AddPolicy("AllowSpecificOrigin", policyBuilder => policyBuilder.WithOrigins("localhost").AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowAllOrigin");
    // app.ApplyMigrations();   // TODO - what package ?
}
else
{
    app.UseCors("AllowSpecificOrigin");
}

app.MapIdentityApi<IdentityUser>();   // used for crude Authorization directly from Identity package (without any custom changes)
// app.MapIdentityApi<User>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
