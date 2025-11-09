using Application.Abstractions.Security;
using Domain.Errors;
using Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.Behaviors;

public sealed class RateLimitingPipelineBehavior<TRequest, TResponse>(
    IRateLimiter rateLimiter,
    IHttpContextAccessor httpContextAccessor
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return await next(cancellationToken);
        }

        // Get client identifier (user ID for authenticated users, IP for anonymous)
        var clientId = GetClientId(httpContext);
        var algorithm = GetAlgorithm(httpContext);

        if (await rateLimiter.IsRateLimitExceededAsync(clientId, algorithm, cancellationToken))
        {
            // Apply penalty for exceeding rate limit
            await rateLimiter.ApplyPenaltyAsync(clientId, cancellationToken);
            
            return (TResponse)Result.Failure(
                DomainErrors.RateLimit.Exceeded);
        }

        return await next(cancellationToken);
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

    private static string GetAlgorithm(HttpContext httpContext)
    {
        // Default to fixed window
        return "fixedwindow";
    }
}
