using Infrastructure.Security;

namespace App.Configurations;

public class RateLimitHeadersServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        // Register the rate limit headers service
        services.AddScoped<RateLimitHeadersService>();
        
        // Register the rate limit monitoring service
        services.AddScoped<RateLimitMonitoringService>();
    }
}
