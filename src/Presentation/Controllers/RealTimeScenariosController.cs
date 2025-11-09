using Microsoft.AspNetCore.Mvc;
using Presentation.Attributes;
using Presentation.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Presentation.Controllers;

[Route("api/real-time")]
public class RealTimeScenariosController : ApiController
{
    public RealTimeScenariosController() : base(null!)
    {
    }

    // Scenario 1: API for a real-time chat application
    // Uses token bucket to allow bursty traffic during active conversations
    [HttpGet("chat-messages")]
    [RateLimit(Algorithm = "tokenbucket", PermitLimit = 100, WindowInSeconds = 10)]
    public IActionResult GetChatMessages([FromQuery] string roomId, [FromQuery] int limit = 50)
    {
        // Simulate fetching chat messages
        var messages = Enumerable.Range(1, Math.Min(limit, 20)).Select(i => new
        {
            id = Guid.NewGuid(),
            roomId,
            userId = $"user{i}",
            message = $"Sample message {i}",
            timestamp = DateTime.UtcNow.AddSeconds(-i * 2)
        });

        return Ok(new { messages, count = messages.Count() });
    }

    // Scenario 2: Financial data API with strict rate limiting
    // Uses fixed window for predictable, consistent limits
    [HttpGet("stock-prices")]
    [RateLimit(Algorithm = "fixedwindow", PermitLimit = 60, WindowInSeconds = 60)]
    public IActionResult GetStockPrices([FromQuery] string[] symbols)
    {
        var prices = symbols.Select(symbol => new
        {
            symbol,
            price = 100 + new Random().NextDouble() * 100,
            change = (new Random().NextDouble() - 0.5) * 10,
            timestamp = DateTime.UtcNow
        });

        return Ok(new { prices, timestamp = DateTime.UtcNow });
    }

    // Scenario 3: File upload endpoint with concurrency limiting
    // Uses concurrency limiter to prevent server overload
    [HttpPost("upload")]
    [RateLimit(Algorithm = "concurrency", PermitLimit = 5)]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        // Simulate file processing
        await Task.Delay(3000);

        return Ok(new
        {
            fileName = file.FileName,
            size = file.Length,
            uploadedAt = DateTime.UtcNow,
            fileId = Guid.NewGuid()
        });
    }

    // Scenario 4: Real-time analytics dashboard
    // Uses sliding window for smooth rate limiting
    [HttpGet("analytics")]
    [RateLimit(Algorithm = "slidingwindow", PermitLimit = 100, WindowInSeconds = 30)]
    public IActionResult GetAnalytics([FromQuery] string dashboardId, [FromQuery] int hours = 24)
    {
        // Generate sample analytics data
        var dataPoints = Enumerable.Range(0, hours).Select(i => new
        {
            timestamp = DateTime.UtcNow.AddHours(-i),
            visitors = new Random().Next(100, 1000),
            pageViews = new Random().Next(500, 5000),
            bounceRate = Math.Round(new Random().NextDouble() * 100, 2)
        });

        return Ok(new
        {
            dashboardId,
            dataPoints = dataPoints.Reverse(), // Most recent first
            totalVisitors = dataPoints.Sum(dp => dp.visitors),
            totalPageViews = dataPoints.Sum(dp => dp.pageViews)
        });
    }

    // Scenario 5: High-priority API for premium users
    // More generous rate limits
    [HttpGet("premium-data")]
    [RateLimit(Algorithm = "tokenbucket", PermitLimit = 1000, WindowInSeconds = 60)]
    public IActionResult GetPremiumData([FromQuery] string dataType)
    {
        return Ok(new
        {
            dataType,
            data = Enumerable.Range(1, 10).Select(i => new
            {
                id = i,
                value = $"Premium data {i}",
                priority = "high",
                timestamp = DateTime.UtcNow
            }),
            accessLevel = "premium"
        });
    }

    // Scenario 6: API gateway with adaptive rate limiting
    // Adjusts limits based on server load (simulated)
    [HttpGet("gateway-data")]
    public IActionResult GetGatewayData()
    {
        // In a real implementation, this would check server metrics
        var serverLoad = new Random().NextDouble();
        var algorithm = serverLoad > 0.8 ? "fixedwindow" : "tokenbucket";
        var permitLimit = serverLoad > 0.8 ? 10 : 100;

        // This demonstrates how the system could adapt to conditions
        return Ok(new
        {
            data = "Gateway data",
            serverLoad = Math.Round(serverLoad, 2),
            appliedRateLimit = new { algorithm, permitLimit },
            timestamp = DateTime.UtcNow
        });
    }
}
