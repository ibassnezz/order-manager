using MediatR;
using OrderManager.Events;

namespace OrderManager.Domain.Storage
{
    public class EventNotification<TKey, TEvent> : INotification where TEvent : IDomainEvent
    {
        public TKey Key { get; }
        public TEvent DomainEvent { get; }

        public EventNotification(TKey key, TEvent domainEvent)
        {
            Key = key;
            DomainEvent = domainEvent;
        }

    }
}
