using Microsoft.Extensions.Options;

namespace App.OptionsSetup;

public class NSwagOptionsSetup(
    IConfiguration configuration
) : IConfigureOptions<NSwagOptions>
{
    private const string SectionName = "NSwag";

    public void Configure(NSwagOptions options)
    {
        configuration.GetSection(SectionName).Bind(options);
    }
}

public class NSwagOptions
{
    public string DocumentName { get; set; } = "v1";
    public string Title { get; set; } = "API Rate Limiter";
    public string Version { get; set; } = "v1";
    public string Description { get; set; } = "API Rate Limiter Service";
    public bool Enabled { get; set; } = true;
}
