using Infrastructure.Security;
using Microsoft.Extensions.Options;

namespace App.Middlewares;

public class RateLimitHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public RateLimitHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, RateLimitHeadersService rateLimitHeadersService)
    {
        // Capture the response body stream
        var originalBodyStream = context.Response.Body;
        
        // Call the next middleware in the pipeline
        await _next(context);
        
        // Add rate limit headers to the response
        // In a real implementation, you would determine the client ID and algorithm used
        var clientId = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var algorithm = "fixedwindow"; // Default algorithm
        
        await rateLimitHeadersService.AddRateLimitHeadersAsync(
            context,
            clientId,
            algorithm,
            context.Response.StatusCode == 429); // Check if rate limited
    }
}