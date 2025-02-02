using App.OptionsSetup;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace App.Configurations;

public class AuthenticationServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        // Configure JWT options setup
        services.ConfigureOptions<JwtOptionsSetup>();
        services.ConfigureOptions<JwtBearerOptionsSetup>();

        // Add authentication services with JWT Bearer authentication scheme
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();
    }
}