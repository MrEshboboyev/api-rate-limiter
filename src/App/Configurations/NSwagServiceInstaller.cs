using App.OptionsSetup;
using Microsoft.Extensions.Options;

namespace App.Configurations;

public class NSwagServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureOptions<NSwagOptionsSetup>();

        using var serviceProvider = services.BuildServiceProvider();
        var nswagOptions = serviceProvider
            .GetRequiredService<IOptions<NSwagOptions>>()
            .Value;

        if (nswagOptions.Enabled)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerDocument(config =>
            {
                config.DocumentName = nswagOptions.DocumentName;
                config.Title = nswagOptions.Title;
                config.Version = nswagOptions.Version;
                config.Description = nswagOptions.Description;
            });
        }
    }
}
