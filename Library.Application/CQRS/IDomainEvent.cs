using MediatR;

namespace Library.Application.CQRS;

public interface IDomainEvent : INotification;

public interface IDomainEventHandler<in TDomainEvent> : INotificationHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent;