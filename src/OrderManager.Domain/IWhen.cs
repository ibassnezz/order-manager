
using OrderManager.Events;

namespace OrderManager.Domain
{
    public interface IWhen<in TEvent> where TEvent : IDomainEvent
    {
        void When(TEvent domainEvent);
    }
}
