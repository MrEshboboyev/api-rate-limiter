namespace Application.Abstractions.Security;

public interface IRateLimiterService
{
    Task<bool> IsRateLimitExceededAsync(string clientId, CancellationToken cancellationToken);
}