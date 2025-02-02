namespace Domain.Events;

public sealed record UserRegisteredDomainEvent(
    Guid Id,
    Guid UserId) : DomainEvent(Id);