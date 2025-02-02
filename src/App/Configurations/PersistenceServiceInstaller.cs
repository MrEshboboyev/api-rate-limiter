using Microsoft.EntityFrameworkCore;
using Persistence;

namespace App.Configurations;

public class PersistenceServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        // Configure the application's DbContext to use SQL Server with the
        // provided connection string
        services.AddDbContext<ApplicationDbContext>(
            options => options.UseNpgsql(
                configuration.GetConnectionString("Database")));
    }
}