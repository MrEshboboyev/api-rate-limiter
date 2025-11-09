using Microsoft.AspNetCore.Mvc;
using Presentation.Attributes;
using Presentation.Abstractions;

namespace Presentation.Controllers;

[Route("api/rate-limit-scenarios")]
public class RateLimitScenariosController : ApiController
{
    public RateLimitScenariosController() : base(null!)
    {
    }

    // Example 1: Different rate limits for different user types
    [HttpGet("premium-user-data")]
    [RateLimit(Algorithm = "tokenbucket", PermitLimit = 1000, WindowInSeconds = 60)]
    public IActionResult GetPremiumUserData()
    {
        return Ok(new { message = "Premium user data", timestamp = DateTime.UtcNow });
    }

    [HttpGet("free-user-data")]
    [RateLimit(Algorithm = "fixedwindow", PermitLimit = 10, WindowInSeconds = 60)]
    public IActionResult GetFreeUserData()
    {
        return Ok(new { message = "Free user data", timestamp = DateTime.UtcNow });
    }

    // Example 2: Real-time API with sliding window
    [HttpGet("real-time-feed")]
    [RateLimit(Algorithm = "slidingwindow", PermitLimit = 50, WindowInSeconds = 10)]
    public IActionResult GetRealTimeFeed()
    {
        return Ok(new { 
            message = "Real-time data feed", 
            timestamp = DateTime.UtcNow,
            data = Enumerable.Range(1, 5).Select(i => new { 
                id = i, 
                value = Guid.NewGuid().ToString()[..8],
                time = DateTime.UtcNow.AddSeconds(-i)
            })
        });
    }

    // Example 3: High-concurrency endpoint
    [HttpPost("bulk-process")]
    [RateLimit(Algorithm = "concurrency", PermitLimit = 20)]
    public async Task<IActionResult> ProcessBulkData([FromBody] List<string> items)
    {
        // Simulate processing time
        await Task.Delay(2000);
        
        return Ok(new { 
            message = $"Processed {items.Count} items", 
            timestamp = DateTime.UtcNow,
            processedItems = items.Select((item, index) => new { 
                original = item, 
                processed = $"{item}_processed",
                index 
            })
        });
    }

    // Example 4: Strict rate limiting for sensitive operations
    [HttpPost("sensitive-operation")]
    [RateLimit(Algorithm = "fixedwindow", PermitLimit = 5, WindowInSeconds = 300)] // 5 requests per 5 minutes
    public IActionResult PerformSensitiveOperation()
    {
        return Ok(new { 
            message = "Sensitive operation completed", 
            timestamp = DateTime.UtcNow 
        });
    }

    // Example 5: Adaptive rate limiting based on server load
    [HttpGet("adaptive-endpoint")]
    public IActionResult GetAdaptiveData()
    {
        // In a real implementation, this would check server load and apply different limits
        var algorithm = DateTime.Now.Second % 2 == 0 ? "tokenbucket" : "slidingwindow";
        
        // This demonstrates how the attribute could be used dynamically
        return Ok(new { 
            message = "Adaptive endpoint response", 
            algorithm,
            timestamp = DateTime.UtcNow 
        });
    }
}
