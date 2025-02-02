using System.Threading.RateLimiting;
using Application;
using Application.Abstractions.Security;
using Application.Behaviors;
using FluentValidation;
using Infrastructure.Security;
using MediatR;

namespace App.Configurations;

public class ApplicationServiceInstaller : IServiceInstaller
{
    /// <summary>
    /// Configures the application services.
    /// </summary>
    /// <param name="services">The collection of services to configure.</param>
    /// <param name="configuration">The application configuration.</param>
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        // Add MediatR services for handling commands and queries
        services.AddMediatR(cfg =>
        {
            // Register handlers from the specified assembly
            cfg.RegisterServicesFromAssembly(AssemblyReference.Assembly);
        });
        
        // Add rate limiting
        services.Configure<RateLimitSettings>(
            configuration.GetSection("RateLimitSettings"));

        
        // Add pipeline behaviors (order matters!)
        
        // Add rate limiting behavior to the pipeline
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RateLimitingPipelineBehavior<,>));

        // Add validation behavior to the pipeline
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

        // Add logging behavior to the pipeline
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));

        // Add FluentValidation validators from the specified assembly
        services.AddValidatorsFromAssembly(
            AssemblyReference.Assembly,
            includeInternalTypes: true);
    }
}