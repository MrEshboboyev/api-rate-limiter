using Domain.Errors;
using Domain.Primitives;
using Domain.Shared;

namespace Domain.ValueObjects;

public sealed class RateLimit : ValueObject
{
    #region Constructors

    private RateLimit(
        int maxRequests,
        TimeSpan timeWindow,
        int penaltyDurationInSeconds)
    {
        MaxRequests = maxRequests;
        TimeWindow = timeWindow;
        PenaltyDurationInSeconds = penaltyDurationInSeconds;
    }
    
    #endregion
    
    #region Properties
    
    public int MaxRequests { get; }
    public TimeSpan TimeWindow { get; }
    public int PenaltyDurationInSeconds { get; }
    
    #endregion

    #region Factory Methods
    
    public static Result<RateLimit> Create(
        int maxRequests,
        TimeSpan timeWindow,
        int penaltyDurationInSeconds = 60)
    {
        if (maxRequests <= 0)
            return Result.Failure<RateLimit>(
                DomainErrors.RateLimit.InvalidMaxRequests(maxRequests));

        if (timeWindow.TotalSeconds <= 0)
            return Result.Failure<RateLimit>(
                DomainErrors.RateLimit.InvalidTimeWindow(timeWindow));

        return Result.Success(new RateLimit(maxRequests, timeWindow, penaltyDurationInSeconds));
    }
    
    #endregion

    #region Overrides
    
    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return MaxRequests;
        yield return TimeWindow;
        yield return PenaltyDurationInSeconds;
    }
    
    #endregion
}