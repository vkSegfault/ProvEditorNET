using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ProvEditorNET.Interfaces;
using ProvEditorNET.Models;
using ProvEditorNET.Repository;
using ProvEditorNET.Services;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddScoped<IGoogleAuth, GoogleAuth>();
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IProvinceService, ProvinceService>();
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
builder.Services.AddDbContext<ProvinceDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
    
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

builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
});

// use simple authorization directly from Indentity package or cutomized authenitaction (needed for SSO)
builder.Services.AddAuthorization(options => 
{
    // policies are just set of Roles ???
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin", "User", "Observer"));  // NEW
    options.AddPolicy("UserPolicy", policy => policy.RequireRole("User", "Observer"));  // NEW
    options.AddPolicy("ObserverPolicy", policy => policy.RequireRole("Observer"));  // NEW
});
builder.Services
    .AddIdentityApiEndpoints<IdentityUser>() // this add /register, /login etc endpoints
    .AddUserManager<UserManager<IdentityUser>>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<IdentityDbContext>(); // this tells to use our DB for storing IdentityUsers and IdentityRoles - SHOULD BE ADDED AS LAST

// alternative when using our custom defined IdentityUser
// builder.Services.AddAuthentication(options =>
//     {
//         options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//         options.DefaultChallengeScheme = BearerTokenDefaults.AuthenticationScheme;
//     })
//     .AddCookie(IdentityConstants.ApplicationScheme)
//     .AddBearerToken(IdentityConstants.BearerScheme);
// builder.Services.AddIdentityCore<User>().AddEntityFrameworkStores<IdentityDbContext>().AddApiEndpoints();

builder.Services.AddAuthentication().AddBearerToken();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigin", policyBuilder => policyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
    options.AddPolicy("AllowSpecificOrigin", policyBuilder => policyBuilder.WithOrigins("localhost").AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddSwaggerGen(c => {
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    c.IgnoreObsoleteActions();
    c.IgnoreObsoleteProperties();
    c.CustomSchemaIds(type => type.FullName);
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

app.MapGroup("api/v1/auth").MapIdentityApi<IdentityUser>();   // used for crude Authorization directly from Identity package (without any custom changes)
// app.MapIdentityApi<User>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
