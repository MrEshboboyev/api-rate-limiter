using Application.Abstractions.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace Presentation.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class RateLimitAttribute : ActionFilterAttribute
{
    public string Algorithm { get; set; } = "fixedwindow";
    public int PermitLimit { get; set; } = -1; // Use default from settings if not specified
    public int WindowInSeconds { get; set; } = -1; // Use default from settings if not specified

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var rateLimiter = context.HttpContext.RequestServices.GetRequiredService<IRateLimiter>();
        
        // Get client identifier (user ID for authenticated users, IP for anonymous)
        var clientId = GetClientId(context.HttpContext);
        
        // Check if rate limit is exceeded
        if (await rateLimiter.IsRateLimitExceededAsync(clientId, Algorithm, context.HttpContext.RequestAborted))
        {
            // Apply penalty for exceeding rate limit
            await rateLimiter.ApplyPenaltyAsync(clientId, context.HttpContext.RequestAborted);
            
            // Return 429 Too Many Requests
            context.Result = new StatusCodeResult(429);
            return;
        }
        
        await next();
    }

    private static string GetClientId(HttpContext httpContext)
    {
        // For authenticated users, use user ID
        if (httpContext.User.Identity?.IsAuthenticated == true)
        {
            return httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
                   httpContext.Connection.RemoteIpAddress?.ToString() ?? 
                   "unknown";
        }
        
        // For anonymous users, use IP address
        return httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}
