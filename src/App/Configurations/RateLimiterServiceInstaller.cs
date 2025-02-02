using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace App.Configurations;

public class RateLimiterServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddRateLimiter(rateLimiterOptions =>
        {
            rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // 🔹 Fixed Window Rate Limiter
            rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
            {
                options.Window = TimeSpan.FromSeconds(10);
                options.PermitLimit = 3;
                options.QueueLimit = 0;
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });

            // 🔹 Sliding Window Rate Limiter
            rateLimiterOptions.AddSlidingWindowLimiter("sliding", options =>
            {
                options.Window = TimeSpan.FromSeconds(15);
                options.SegmentsPerWindow = 3;
                options.PermitLimit = 15;
            });

            // 🔹 Token Bucket Rate Limiter
            rateLimiterOptions.AddTokenBucketLimiter("token", options =>
            {
                options.TokenLimit = 100;
                options.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
                options.TokensPerPeriod = 10;
            });

            // 🔹 Concurrency Rate Limiter
            rateLimiterOptions.AddConcurrencyLimiter("concurrency", options =>
            {
                options.PermitLimit = 5;
            });

            // ✅ Custom Policy Based on Client IP
            rateLimiterOptions.AddPolicy("fixed-ip", httpContext =>
            {
                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
                    // partitionKey: httpContext.User.Identity?.Name?.ToString(),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 10,
                        Window = TimeSpan.FromSeconds(10)
                    });
            });
        });
    }
}
