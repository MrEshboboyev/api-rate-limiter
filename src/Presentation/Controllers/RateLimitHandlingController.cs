using Microsoft.AspNetCore.Mvc;
using Presentation.Attributes;
using Presentation.Abstractions;

namespace Presentation.Controllers;

[Route("api/rate-limit-handling")]
public class RateLimitHandlingController : ApiController
{
    public RateLimitHandlingController() : base(null!)
    {
    }

    // Example of an endpoint with very strict rate limiting for sensitive operations
    [HttpPost("delete-account")]
    [RateLimit(Algorithm = "fixedwindow", PermitLimit = 3, WindowInSeconds = 3600)] // 3 times per hour
    public IActionResult DeleteAccount([FromBody] DeleteAccountRequest request)
    {
        // In a real implementation, this would perform the account deletion
        return Ok(new
        {
            message = "Account deletion scheduled",
            accountId = request.AccountId,
            scheduledAt = DateTime.UtcNow,
            confirmation = Guid.NewGuid()
        });
    }

    // Example of an endpoint with adaptive rate limiting based on user tier
    [HttpGet("user-data")]
    public IActionResult GetUserData([FromQuery] string userId, [FromQuery] string userTier = "free")
    {
        // Determine rate limiting based on user tier
        var (algorithm, permitLimit, windowInSeconds) = userTier.ToLower() switch
        {
            "premium" => ("tokenbucket", 1000, 60),
            "pro" => ("slidingwindow", 100, 60),
            _ => ("fixedwindow", 10, 60)
        };

        // In a real implementation, this would fetch user data
        return Ok(new
        {
            userId,
            userTier,
            data = new { name = "John Doe", email = "john@example.com" },
            rateLimit = new { algorithm, permitLimit, windowInSeconds }
        });
    }

    // Example of an endpoint that implements exponential backoff
    [HttpGet("retry-example")]
    [RateLimit(Algorithm = "fixedwindow", PermitLimit = 5, WindowInSeconds = 60)]
    public IActionResult RetryExample()
    {
        return Ok(new
        {
            message = "Success",
            timestamp = DateTime.UtcNow,
            retryInfo = new
            {
                maxRetries = 3,
                backoffMultiplier = 2,
                initialDelayMs = 1000
            }
        });
    }
}

public class DeleteAccountRequest
{
    public string AccountId { get; set; } = string.Empty;
    public string ConfirmationCode { get; set; } = string.Empty;
}
