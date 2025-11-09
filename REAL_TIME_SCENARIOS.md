# Real-Time Scenarios Using Enhanced Rate Limiting Features

This document demonstrates how to use the enhanced rate limiting features in real-world scenarios.

## 1. Different Rate Limits for Different User Types

```csharp
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
```

## 2. Real-Time API with Sliding Window

```csharp
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
```

## 3. High-Concurrency Endpoint

```csharp
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
```

## 4. Strict Rate Limiting for Sensitive Operations

```csharp
[HttpPost("sensitive-operation")]
[RateLimit(Algorithm = "fixedwindow", PermitLimit = 5, WindowInSeconds = 300)] // 5 requests per 5 minutes
public IActionResult PerformSensitiveOperation()
{
    return Ok(new { 
        message = "Sensitive operation completed", 
        timestamp = DateTime.UtcNow 
    });
}
```

## 5. Real-Time Chat Application

```csharp
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
```

## 6. Financial Data API

```csharp
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
```

## 7. File Upload Endpoint

```csharp
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
```

## 8. Real-Time Analytics Dashboard

```csharp
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
```

## 9. Rate Limit Monitoring

```csharp
[HttpGet("client/{clientId}")]
public async Task<IActionResult> GetClientMetrics(string clientId, [FromQuery] string algorithm = "fixedwindow")
{
    var metrics = await _monitoringService.GetRateLimitMetricsAsync(clientId, algorithm);
    return Ok(metrics);
}
```

## 10. Rate Limit Headers in Responses

The system automatically adds the following headers to HTTP responses:
- `X-RateLimit-Limit`: The maximum number of requests allowed
- `X-RateLimit-Remaining`: The number of requests remaining
- `X-RateLimit-Reset`: The time at which the rate limit will reset
- `X-RateLimit-Algorithm`: The algorithm used for rate limiting
- `X-RateLimit-Client`: The client identifier
- `Retry-After`: Seconds to wait before making another request (when rate limited)

## Key Features Demonstrated

1. **Multiple Algorithms**: Fixed Window, Sliding Window, Token Bucket, and Concurrency
2. **Granular Control**: Different limits for different endpoints and user types
3. **Real-Time Monitoring**: Track rate limit usage and metrics
4. **Adaptive Rate Limiting**: Adjust limits based on server load or user tier
5. **Comprehensive Headers**: Rich information in HTTP response headers
6. **Penalty/Ban System**: Automatic penalties and bans for abusive clients
7. **Distributed Support**: Redis-based implementation for scalability
8. **Authentication Awareness**: Different limits for authenticated vs anonymous users

These examples show how the enhanced rate limiting system can be used in various real-world scenarios to provide robust, scalable, and fair API access control.