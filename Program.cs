using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using ProvEditorNET.Extensions;
using ProvEditorNET.Interfaces;
using ProvEditorNET.Models;
using ProvEditorNET.Repository;
using ProvEditorNET.Services;
using Scalar.AspNetCore;
using StackExchange.Redis;
using ProvEditorNET.Helpers;



var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddScoped<IGoogleAuth, GoogleAuth>();
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IRedisService, RedisService>();
builder.Services.AddScoped<IProvinceService, ProvinceService>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IResourceService, ResourceService>();
builder.Services.AddScoped<IInfrastructureService, InfrastructureService>();

// builder.Services.AddEndpointsApiExplorer();

// now Open API specs are here: http://localhost:5077/openapi/v1.json
builder.Services.AddOpenApi("v1", options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

#if(DEBUG)
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
#else
string connectionString = builder.Configuration.GetConnectionString("DockerConnection");
#endif

var serviceCollection = builder.Services.AddDbContext<IdentityDbContext>(options =>
{
    options.UseNpgsql( connectionString );
});
builder.Services.AddDbContext<ProvinceDbContext>(options =>
{
    options.UseNpgsql( connectionString );
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisLocalConnection");
    options.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions()
    {
        AbortOnConnectFail = true, // terminate app if can't connect to Redis instance
        EndPoints = { options.Configuration }  // ensure endpoint is set correctly
    };
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
    // policies are just set of Roles
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin", "User", "Observer"));  // NEW
    options.AddPolicy("UserPolicy", policy => policy.RequireRole("User", "Observer"));  // NEW
    options.AddPolicy("ObserverPolicy", policy => policy.RequireRole("Observer"));  // NEW
});
builder.Services
    .AddIdentityApiEndpoints<IdentityUser>() // this add /register, /login etc endpoints
    .AddUserManager<UserManager<IdentityUser>>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<IdentityDbContext>(); // this tells to use our DB for storing IdentityUsers and IdentityRoles - SHOULD BE ADDED AS LAST

// this only applies to Bearer Token that is not stored as cookie in browser (cookie and session cookie TTLs needs to be configured elsewhere):
builder.Services.AddOptions<BearerTokenOptions>(IdentityConstants.BearerScheme).Configure(
    options =>
    {
        options.BearerTokenExpiration = TimeSpan.FromMinutes(20);
    }
    );


// for setting cookies and session cookies reference:
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-8.0

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
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigin", policyBuilder => policyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
    options.AddPolicy("AllowSpecificOrigin", policyBuilder => policyBuilder.WithOrigins("localhost").AllowAnyMethod().AllowAnyHeader());
});


// Open Telemetry, Promotheus, Grafana - data provided by NET Core itself
builder.Services.AddOpenTelemetry()
    .WithMetrics(x =>
    {
        x.AddMeter("Microsoft.AspNetCore.Hosting");
        x.AddMeter("Microsoft.AspNetCore.Server.Kestrel");
        x.AddMeter("CountryAddedMeter");
        x.AddView("request-duration",
            new ExplicitBucketHistogramConfiguration
            {
                Boundaries = new [] { 0, 0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75 }
            });
        x.AddPrometheusExporter();  // export dotnet app data to Prometheus
    });

// add our custom matrics defined in endpoints
builder.Services.AddMetrics();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Prov Editor API";
        options.WithTheme(ScalarTheme.BluePlanet);
        options.DefaultHttpClient = new(ScalarTarget.Shell, ScalarClient.HttpClient);
        // options.WithPreferredScheme("Bearer")
        // options.WithHttpBearerAuthentication(bearer => { bearer.Token = "YOUR_BEARER_TOKEN"; });
        // options.Authentication = new ScalarAuthenticationOptions
        // {
        //     PreferredSecurityScheme = "Bearer",
        //     ApiKey = new ApiKeyOptions
        //     {
        //         
        //     }
        // };
    });
    // app.UseSwagger();
    // app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Swagger"));   // we can still use Swagger UI but with natively generated by dotnet 9 Open API specs
    app.UseCors("AllowAllOrigin");
    app.ApplyMigrations();   // Never do this in Production (https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying?tabs=dotnet-core-cli#apply-migrations-at-runtime)
}
else
{
    // TODO - disable Swagger in prod/Release builds
    // app.UseSwagger();
    // app.UseSwaggerUI();
    app.UseCors("AllowSpecificOrigin");
    // app.UseCors("AllowAllOrigin");
}


app.MapPrometheusScrapingEndpoint();  // expose endpoint for Promoetheus (http://<HOST>:<PORT>/metrics)
app.MapGroup("api/v1/auth").MapIdentityApi<IdentityUser>();   // used for crude Authorization directly from Identity package (without any custom changes)
// app.MapIdentityApi<User>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
