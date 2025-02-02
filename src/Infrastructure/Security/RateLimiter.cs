using Application.Abstractions.Security;
using StackExchange.Redis;

namespace Infrastructure.Security;

internal sealed class RateLimiter(
    IConnectionMultiplexer redis, // Injected Redis connection
    RateLimitSettings settings)
    : IRateLimiter
{
    private readonly IDatabase _redisDb = redis.GetDatabase();

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
                TimeSpan.FromMinutes(settings.WindowInMinutes));
        }

        return currentCount > settings.PermitLimit;
    }
}