namespace Application.Abstractions.Security;

public interface IRateLimiter
{
    Task<bool> IsRateLimitExceededAsync(
        string clientId, 
        string algorithm,
        CancellationToken cancellationToken);
    
    Task ApplyPenaltyAsync(
        string clientId, 
        CancellationToken cancellationToken);
}
