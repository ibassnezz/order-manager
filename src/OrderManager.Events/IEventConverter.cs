namespace OrderManager.Events
{
    public interface IEventConverter<out TEventTo> 
        where TEventTo : IDomainEvent
    {
        TEventTo Convert(IDomainEvent eventFrom);
    }


    public abstract class AbstractEventConverter<TEventFrom, TEventTo> : IEventConverter<IDomainEvent>
        where TEventFrom : IDomainEvent
        where TEventTo : IDomainEvent
    {

        public abstract TEventTo Convert(TEventFrom eventFrom);

        public IDomainEvent Convert(IDomainEvent eventFrom)
        {
            return Convert((TEventFrom)eventFrom);
        }

    }
    
}
