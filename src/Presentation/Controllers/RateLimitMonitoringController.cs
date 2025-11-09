using Infrastructure.Security;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstractions;
using MediatR;

namespace Presentation.Controllers;

[Route("api/rate-limit-monitoring")]
public class RateLimitMonitoringController(
    ISender sender,
    RateLimitMonitoringService monitoringService
) : ApiController(sender)
{
    // Get metrics for a specific client
    [HttpGet("client/{clientId}")]
    public async Task<IActionResult> GetClientMetrics(string clientId, [FromQuery] string algorithm = "fixedwindow")
    {
        var metrics = await monitoringService.GetRateLimitMetricsAsync(clientId, algorithm);
        return Ok(metrics);
    }

    // Get top rate-limited clients
    [HttpGet("top-clients")]
    public async Task<IActionResult> GetTopRateLimitedClients([FromQuery] int count = 10)
    {
        var clients = await monitoringService.GetTopRateLimitedClientsAsync(count);
        return Ok(clients);
    }

    // Get system-wide rate limiting statistics
    [HttpGet("statistics")]
    public IActionResult GetRateLimitStatistics()
    {
        // In a real implementation, this would aggregate metrics from Redis
        var statistics = new
        {
            totalRequests = new Random().Next(1000, 10000),
            rateLimitedRequests = new Random().Next(10, 100),
            rateLimitPercentage = Math.Round(new Random().NextDouble() * 5, 2),
            topAlgorithms = new[]
            {
                new { algorithm = "FixedWindow", requests = new Random().Next(300, 1000) },
                new { algorithm = "SlidingWindow", requests = new Random().Next(200, 800) },
                new { algorithm = "TokenBucket", requests = new Random().Next(100, 600) },
                new { algorithm = "Concurrency", requests = new Random().Next(50, 300) }
            },
            timestamp = DateTime.UtcNow
        };

        return Ok(statistics);
    }
}
