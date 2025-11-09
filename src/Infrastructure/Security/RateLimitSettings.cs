namespace Infrastructure.Security;

public sealed class RateLimitSettings
{
    public FixedWindowSettings FixedWindow { get; set; } = new();
    public SlidingWindowSettings SlidingWindow { get; set; } = new();
    public TokenBucketSettings TokenBucket { get; set; } = new();
    public ConcurrencySettings Concurrency { get; set; } = new();
    public GlobalSettings Global { get; set; } = new();
}

public class FixedWindowSettings
{
    public int PermitLimit { get; set; } = 100;
    public int WindowInSeconds { get; set; } = 60;
}

public class SlidingWindowSettings
{
    public int PermitLimit { get; set; } = 100;
    public int WindowInSeconds { get; set; } = 60;
    public int SegmentsPerWindow { get; set; } = 6;
}

public class TokenBucketSettings
{
    public int TokenLimit { get; set; } = 100;
    public int TokensPerPeriod { get; set; } = 10;
    public int ReplenishmentPeriodInSeconds { get; set; } = 10;
}

public class ConcurrencySettings
{
    public int PermitLimit { get; set; } = 10;
}

public class GlobalSettings
{
    public int PenaltyDurationInSeconds { get; set; } = 60;
    public int MaxPenaltyDurationInSeconds { get; set; } = 300;
    public int BanThreshold { get; set; } = 5;
    public int BanDurationInSeconds { get; set; } = 3600; // 1 hour
}
