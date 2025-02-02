namespace App.Configurations;

public class AuthorizationServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        // Add authorization services
        services.AddAuthorization();
    }
}