using Application.Abstractions.Security;
using Microsoft.Extensions.Options;
using Serilog;
using StackExchange.Redis;

namespace Infrastructure.Security;

internal sealed class RateLimiter(
    IConnectionMultiplexer redis,
    IOptions<RateLimitSettings> options
) : IRateLimiter
{
    private readonly IDatabase _redisDb = redis.GetDatabase();
    private readonly RateLimitSettings _settings = options.Value;
    private readonly ILogger _logger = Log.ForContext<RateLimiter>();

    public async Task<bool> IsRateLimitExceededAsync(
        string clientId,
        string algorithm,
        CancellationToken cancellationToken)
    {
        try
        {
            // Check if client is banned
            var banKey = $"ban:{clientId}";
            var isBanned = await _redisDb.KeyExistsAsync(banKey);
            if (isBanned)
            {
                _logger.Warning("Rate limit check failed for banned client {ClientId}", clientId);
                return true;
            }

            var result = algorithm.ToLower() switch
            {
                "fixedwindow" => await IsFixedWindowLimitExceededAsync(clientId, cancellationToken),
                "slidingwindow" => await IsSlidingWindowLimitExceededAsync(clientId, cancellationToken),
                "tokenbucket" => await IsTokenBucketLimitExceededAsync(clientId, cancellationToken),
                "concurrency" => await IsConcurrencyLimitExceededAsync(clientId, cancellationToken),
                _ => await IsFixedWindowLimitExceededAsync(clientId, cancellationToken)
            };

            _logger.Information("Rate limit check for client {ClientId} with algorithm {Algorithm}: {Result}", 
                clientId, algorithm, result ? "Exceeded" : "Allowed");

            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error checking rate limit for client {ClientId}", clientId);
            // Fail open - allow request through if there's an error
            return false;
        }
    }

    private async Task<bool> IsFixedWindowLimitExceededAsync(
        string clientId,
        CancellationToken cancellationToken)
    {
        var key = $"rate_limit:fixed:{clientId}";
        var currentCount = await _redisDb.StringIncrementAsync(key);

        if (currentCount == 1)
        {
            await _redisDb.KeyExpireAsync(
                key,
                TimeSpan.FromSeconds(_settings.FixedWindow.WindowInSeconds));
        }

        return currentCount > _settings.FixedWindow.PermitLimit;
    }

    private async Task<bool> IsSlidingWindowLimitExceededAsync(
        string clientId,
        CancellationToken cancellationToken)
    {
        var key = $"rate_limit:sliding:{clientId}";
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var windowStart = now - _settings.SlidingWindow.WindowInSeconds;
        
        // Add current request to sorted set
        await _redisDb.SortedSetAddAsync(key, now.ToString(), now);
        
        // Remove old entries outside the window
        await _redisDb.SortedSetRemoveRangeByScoreAsync(key, 0, windowStart);
        
        // Set expiration
        await _redisDb.KeyExpireAsync(
            key,
            TimeSpan.FromSeconds(_settings.SlidingWindow.WindowInSeconds + 10));
        
        // Get count
        var count = await _redisDb.SortedSetLengthAsync(key);
        
        return count > _settings.SlidingWindow.PermitLimit;
    }

    private async Task<bool> IsTokenBucketLimitExceededAsync(
        string clientId,
        CancellationToken cancellationToken)
    {
        var key = $"rate_limit:token:{clientId}";
        var lastRefillKey = $"rate_limit:token:last_refill:{clientId}";
        
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var lastRefill = (long)await _redisDb.StringGetAsync(lastRefillKey);
        
        if (lastRefill == 0)
        {
            lastRefill = now;
            await _redisDb.StringSetAsync(lastRefillKey, lastRefill);
        }
        
        // Calculate tokens to add
        var secondsSinceLastRefill = now - lastRefill;
        var tokensToAdd = (secondsSinceLastRefill / _settings.TokenBucket.ReplenishmentPeriodInSeconds) * 
                          _settings.TokenBucket.TokensPerPeriod;
        
        // Get current tokens
        var currentTokens = (double)await _redisDb.StringGetAsync(key);
        if (currentTokens == 0)
        {
            currentTokens = _settings.TokenBucket.TokenLimit;
        }
        
        // Refill tokens
        var newTokens = Math.Min(_settings.TokenBucket.TokenLimit, currentTokens + tokensToAdd);
        
        // Try to consume one token
        if (newTokens >= 1)
        {
            await _redisDb.StringSetAsync(key, newTokens - 1);
            await _redisDb.StringSetAsync(lastRefillKey, now);
            return false;
        }
        
        // Not enough tokens
        await _redisDb.StringSetAsync(key, newTokens);
        await _redisDb.StringSetAsync(lastRefillKey, now);
        return true;
    }

    private async Task<bool> IsConcurrencyLimitExceededAsync(
        string clientId,
        CancellationToken cancellationToken)
    {
        var key = $"rate_limit:concurrency:{clientId}";
        var currentCount = await _redisDb.StringIncrementAsync(key);
        
        if (currentCount == 1)
        {
            await _redisDb.KeyExpireAsync(
                key,
                TimeSpan.FromMinutes(1)); // Expire after 1 minute of inactivity
        }
        
        if (currentCount > _settings.Concurrency.PermitLimit)
        {
            await _redisDb.StringDecrementAsync(key);
            return true;
        }
        
        // Register cleanup action
        _ = Task.Run(async () =>
        {
            await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            await _redisDb.StringDecrementAsync(key);
        }, cancellationToken);
        
        return false;
    }

    public async Task ApplyPenaltyAsync(string clientId, CancellationToken cancellationToken)
    {
        try
        {
            var penaltyKey = $"penalty:{clientId}";
            var currentPenalty = await _redisDb.StringIncrementAsync(penaltyKey);
            
            if (currentPenalty == 1)
            {
                await _redisDb.KeyExpireAsync(
                    penaltyKey,
                    TimeSpan.FromSeconds(_settings.Global.PenaltyDurationInSeconds));
            }
            else
            {
                // Increase penalty duration exponentially, up to a maximum
                var newDuration = Math.Min(
                    _settings.Global.PenaltyDurationInSeconds * (int)currentPenalty,
                    _settings.Global.MaxPenaltyDurationInSeconds);
                
                await _redisDb.KeyExpireAsync(
                    penaltyKey,
                    TimeSpan.FromSeconds(newDuration));
            }
            
            // Check if ban threshold is reached
            if (currentPenalty >= _settings.Global.BanThreshold)
            {
                var banKey = $"ban:{clientId}";
                await _redisDb.StringSetAsync(banKey, "banned", 
                    TimeSpan.FromSeconds(_settings.Global.BanDurationInSeconds));
                
                // Remove penalty key as client is now banned
                await _redisDb.KeyDeleteAsync(penaltyKey);
                
                _logger.Warning("Client {ClientId} has been banned after {PenaltyCount} penalties", 
                    clientId, currentPenalty);
            }
            else
            {
                _logger.Information("Applied penalty #{PenaltyCount} to client {ClientId}", 
                    currentPenalty, clientId);
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error applying penalty to client {ClientId}", clientId);
        }
    }
}
