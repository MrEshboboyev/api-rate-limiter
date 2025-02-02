using Domain.Primitives;

namespace Domain.Events;

public abstract record DomainEvent(Guid Id) : IDomainEvent;