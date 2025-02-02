using Application.Abstractions.Security;
using Domain.Errors;
using Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Behaviors;

public sealed class RateLimitingPipelineBehavior<TRequest, TResponse>(
    IRateLimiter rateLimiter,
    IHttpContextAccessor httpContextAccessor)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Get client IP (or user ID for authenticated users)
        var clientId = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() 
                       ?? "unknown";

        if (await rateLimiter.IsRateLimitExceededAsync(clientId, cancellationToken))
        {
            return (TResponse)Result.Failure(
                DomainErrors.RateLimit.Exceeded);
        }

        return await next();
    }
}