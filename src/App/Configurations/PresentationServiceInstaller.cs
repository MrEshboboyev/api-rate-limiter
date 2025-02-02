using Presentation;

namespace App.Configurations;

public class PresentationServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        // Add controllers and application part
        services
            .AddControllers()
            .AddApplicationPart(AssemblyReference.Assembly);

        // Add OpenAPI support
        services.AddOpenApi();
    }
}