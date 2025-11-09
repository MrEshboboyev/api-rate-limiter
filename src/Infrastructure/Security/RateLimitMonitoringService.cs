using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Serilog;

namespace Infrastructure.Security;

public class RateLimitMonitoringService(
    IConnectionMultiplexer redis,
    IOptions<RateLimitSettings> settings
)
{
    private readonly IDatabase _redisDb = redis.GetDatabase();
    private readonly RateLimitSettings _settings = settings.Value;
    private readonly ILogger _logger = Log.ForContext<RateLimitMonitoringService>();

    public async Task<RateLimitMetrics> GetRateLimitMetricsAsync(string clientId, string algorithm)
    {
        var key = $"rate_limit:{algorithm.ToLower()}:{clientId}";
        var currentCount = await _redisDb.StringGetAsync(key);
        
        var limit = GetLimitForAlgorithm(algorithm);
        var remaining = limit - (int)currentCount;
        
        var isRateLimited = remaining < 0;
        
        var metrics = new RateLimitMetrics
        {
            ClientId = clientId,
            Algorithm = algorithm,
            CurrentRequests = (int)currentCount,
            Limit = limit,
            Remaining = Math.Max(0, remaining),
            IsRateLimited = isRateLimited,
            ResetTime = DateTime.UtcNow.AddSeconds(GetWindowForAlgorithm(algorithm))
        };
        
        _logger.Information(
            "Rate limit metrics for client {ClientId} with algorithm {Algorithm}: {@Metrics}",
            clientId, algorithm, metrics);
        
        return metrics;
    }

    public async Task<Dictionary<string, RateLimitMetrics>> GetTopRateLimitedClientsAsync(int count = 10)
    {
        // In a real implementation, this would scan Redis for keys and aggregate metrics
        // This is a simplified example
        var clients = new Dictionary<string, RateLimitMetrics>();
        
        // Simulate getting top clients
        for (int i = 1; i <= count; i++)
        {
            var clientId = $"client-{i}";
            var algorithm = (i % 4) switch
            {
                0 => "fixedwindow",
                1 => "slidingwindow",
                2 => "tokenbucket",
                _ => "concurrency"
            };
            
            clients[clientId] = await GetRateLimitMetricsAsync(clientId, algorithm);
        }
        
        return clients;
    }

    private int GetLimitForAlgorithm(string algorithm)
    {
        return algorithm.ToLower() switch
        {
            "fixedwindow" => _settings.FixedWindow.PermitLimit,
            "slidingwindow" => _settings.SlidingWindow.PermitLimit,
            "tokenbucket" => _settings.TokenBucket.TokenLimit,
            "concurrency" => _settings.Concurrency.PermitLimit,
            _ => _settings.FixedWindow.PermitLimit
        };
    }

    private int GetWindowForAlgorithm(string algorithm)
    {
        return algorithm.ToLower() switch
        {
            "fixedwindow" => _settings.FixedWindow.WindowInSeconds,
            "slidingwindow" => _settings.SlidingWindow.WindowInSeconds,
            "tokenbucket" => _settings.TokenBucket.ReplenishmentPeriodInSeconds,
            "concurrency" => 60,
            _ => _settings.FixedWindow.WindowInSeconds
        };
    }
}

public class RateLimitMetrics
{
    public string ClientId { get; set; } = string.Empty;
    public string Algorithm { get; set; } = string.Empty;
    public int CurrentRequests { get; set; }
    public int Limit { get; set; }
    public int Remaining { get; set; }
    public bool IsRateLimited { get; set; }
    public DateTime ResetTime { get; set; }
}
