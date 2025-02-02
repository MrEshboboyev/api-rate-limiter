using App.Middlewares;

namespace App.Configurations;

public class MiddlewaresServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        // Register the GlobalExceptionHandlingMiddleware as a transient service
        services.AddTransient<GlobalExceptionHandlingMiddleware>();
    }
}