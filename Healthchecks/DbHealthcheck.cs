using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProvEditorNET.Repository;

namespace ProvEditorNET.Healthchecks;

// Classical Load Balancer will look for healthchecks under "_health" endpoint
// once this endpoint is invoked all healthchecks here will be executed

public class DbHealthcheck : IHealthCheck
{
    private readonly ProvinceDbContext _context;
    private readonly ILogger<DbHealthcheck> _logger;

    public DbHealthcheck(ProvinceDbContext context, ILogger<DbHealthcheck> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        bool canConnect = await _context.Database.CanConnectAsync(cancellationToken);
        if (canConnect)
        {
            return HealthCheckResult.Healthy();
        }
        else
        {
            _logger.LogError("Can't connect to DB");
            return HealthCheckResult.Unhealthy("Can't connect to DB");
        }
    }
}