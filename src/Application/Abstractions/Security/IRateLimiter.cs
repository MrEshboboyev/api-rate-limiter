namespace Application.Abstractions.Security;

public interface IRateLimiter
{
    Task<bool> IsRateLimitExceededAsync(string clientId, CancellationToken cancellationToken);
}