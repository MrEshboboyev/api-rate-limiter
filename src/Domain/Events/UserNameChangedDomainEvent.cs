namespace Domain.Events;

public sealed record UserNameChangedDomainEvent(
    Guid Id, 
    Guid UserId) : DomainEvent(Id);