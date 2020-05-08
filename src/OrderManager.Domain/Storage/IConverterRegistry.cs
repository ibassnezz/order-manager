using OrderManager.Events;

namespace OrderManager.Domain.Storage
{
    public interface IConverterRegistry
    {
        IDomainEvent Convert<TEventFrom>(TEventFrom eventFrom)
            where TEventFrom : IDomainEvent;
    }
}