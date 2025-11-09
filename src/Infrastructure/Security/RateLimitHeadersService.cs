using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Globalization;
using Serilog;

namespace Infrastructure.Security;

public class RateLimitHeadersService(
    IConnectionMultiplexer redis,
    IOptions<RateLimitSettings> settings)
{
    private readonly IDatabase _redisDb = redis.GetDatabase();
    private readonly RateLimitSettings _settings = settings.Value;
    private readonly ILogger _logger = Log.ForContext<RateLimitHeadersService>();

    public async Task AddRateLimitHeadersAsync(
        HttpContext context,
        string clientId,
        string algorithm,
        bool isRateLimited)
    {
        try
        {
            // Check if the response has already started
            if (context.Response.HasStarted)
            {
                _logger.Warning("Cannot add rate limit headers because the response has already started");
                return;
            }

            // Add standard rate limit headers
            context.Response.Headers["X-RateLimit-Limit"] = GetLimitForAlgorithm(algorithm).ToString(CultureInfo.InvariantCulture);
            context.Response.Headers["X-RateLimit-Remaining"] = (await GetRemainingRequestsAsync(clientId, algorithm)).ToString(CultureInfo.InvariantCulture);
            context.Response.Headers["X-RateLimit-Reset"] = GetResetTime(clientId, algorithm);
            
            // Add algorithm-specific header
            context.Response.Headers["X-RateLimit-Algorithm"] = algorithm;
            
            // Add client identifier
            context.Response.Headers["X-RateLimit-Client"] = clientId;
            
            // If rate limited, add retry-after header
            if (isRateLimited)
            {
                var retryAfter = await GetRetryAfterAsync(clientId, algorithm);
                context.Response.Headers["Retry-After"] = retryAfter.ToString(CultureInfo.InvariantCulture);
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error adding rate limit headers");
        }
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

    private async Task<int> GetRemainingRequestsAsync(string clientId, string algorithm)
    {
        var key = $"rate_limit:{algorithm.ToLower()}:{clientId}";
        var currentCount = await _redisDb.StringGetAsync(key);
        
        var limit = GetLimitForAlgorithm(algorithm);
        var remaining = limit - (int)currentCount;
        
        return Math.Max(0, remaining);
    }

    private string GetResetTime(string clientId, string algorithm)
    {
        // Calculate based on algorithm
        var windowInSeconds = algorithm.ToLower() switch
        {
            "fixedwindow" => _settings.FixedWindow.WindowInSeconds,
            "slidingwindow" => _settings.SlidingWindow.WindowInSeconds,
            "tokenbucket" => _settings.TokenBucket.ReplenishmentPeriodInSeconds,
            "concurrency" => 60, // Concurrency doesn't have a time window
            _ => _settings.FixedWindow.WindowInSeconds
        };
        
        return DateTimeOffset.UtcNow.AddSeconds(windowInSeconds).ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);
    }

    private async Task<int> GetRetryAfterAsync(string clientId, string algorithm)
    {
        // For now, we'll use a fixed value based on the algorithm
        return await Task.FromResult(algorithm.ToLower() switch
        {
            "fixedwindow" => _settings.FixedWindow.WindowInSeconds,
            "slidingwindow" => _settings.SlidingWindow.WindowInSeconds / _settings.SlidingWindow.SegmentsPerWindow,
            "tokenbucket" => _settings.TokenBucket.ReplenishmentPeriodInSeconds,
            "concurrency" => 10, // Concurrency limits reset quickly
            _ => 60
        });
    }
}