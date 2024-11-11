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

// use simple authorization directly from Indentity package or cutomized authenitaction (needed for SSO)
// builder.Services.AddAuthorization();
// builder.Services.AddIdentityApiEndpoints<IdentityUser>().AddEntityFrameworkStores<IdentityDbContext>();

// alternative when using our custom defined IdentityUser
builder.Services.AddAuthentication().AddCookie(IdentityConstants.ApplicationScheme).AddBearerToken(IdentityConstants.BearerScheme);
builder.Services.AddIdentityCore<User>().AddEntityFrameworkStores<IdentityDbContext>().AddApiEndpoints();

// Google SSO
builder.Services.AddAuthentication().AddGoogle(googleOptions =>
{
    googleOptions.ClientId = "592195124025-6g06a3tddd3fpu494rsrplopn83f7jb2.apps.googleusercontent.com";
    googleOptions.ClientSecret = "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";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // app.ApplyMigrations();   // TODO - what package ?
}

// app.MapIdentityApi<IdentityUser>();   // used for crude Authorization directly from Identity package (without any custom changes)
app.MapIdentityApi<User>();
app.UseHttpsRedirection();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
