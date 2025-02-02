namespace Domain.Events;

public sealed record UserExceededRateLimitDomainEvent(
    Guid Id, 
    Guid UserId,
    DateTime OccurredOn,
    int MaxRequests,
    TimeSpan TimeWindow,
    int PenaltyDurationInSeconds) : DomainEvent(Id);