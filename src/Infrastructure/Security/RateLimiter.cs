using Application.Abstractions.Security;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Infrastructure.Security;

internal sealed class RateLimiter(
    IConnectionMultiplexer redis, // Injected Redis connection
    IOptions<RateLimitSettings> options)
    : IRateLimiter
{
    private readonly IDatabase _redisDb = redis.GetDatabase();
    private readonly RateLimitSettings _settings = options.Value;

    public async Task<bool> IsRateLimitExceededAsync(
        string clientId,
        CancellationToken cancellationToken)
    {
        var key = $"rate_limit:{clientId}";
        var currentCount = await _redisDb.StringIncrementAsync(key);

        // Set expiration on first request
        if (currentCount == 1)
        {
            await _redisDb.KeyExpireAsync(
                key, 
                // TimeSpan.FromMinutes(_settings.WindowInMinutes)
                TimeSpan.FromSeconds(_settings.WindowInSeconds));
        }

        return currentCount > _settings.PermitLimit;
    }
}