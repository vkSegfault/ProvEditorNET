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
builder.Services.AddAuthorization(Options => 
{
    // policies are just set of Roles ???
    Options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin", "User", "Observer"));  // NEW
    Options.AddPolicy("UserPolicy", policy => policy.RequireRole("User", "Observer"));  // NEW
    Options.AddPolicy("ObserverPolicy", policy => policy.RequireRole("Observer"));  // NEW
});
builder.Services
    .AddIdentity<IdentityUser, IdentityRole>()  // NEW
    .AddRoles<IdentityRole>()
    .AddIdentityApiEndpoints<IdentityUser>() // this add /register, /login etc endpoints
    .AddIdentityApiEndpoints<IdentityRole>()  // needed ? for any new roles endpoint? if it adds new endpoint then 
    .AddEntityFrameworkStores<IdentityDbContext>() // this tells to use our DB for storing identity users
    .AddUserManager<UserManager<IdentityUser>>();

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

app.MapIdentityApi<IdentityUser>();   // used for crude Authorization directly from Identity package (without any custom changes)
// app.MapIdentityApi<User>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
