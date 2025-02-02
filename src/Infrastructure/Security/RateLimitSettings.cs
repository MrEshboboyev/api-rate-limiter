namespace Infrastructure.Security;

public sealed class RateLimitSettings
{
    public int PermitLimit { get; set; } // Requests per window (e.g., 100)
    public int WindowInMinutes { get; set; } // Time window (e.g., 1 minute)
    public int WindowInSeconds { get; set; } // Time window (e.g., 1 minute)
}