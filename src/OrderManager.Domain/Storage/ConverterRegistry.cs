using System;
using System.Collections.Generic;
using OrderManager.Events;

namespace OrderManager.Domain.Storage
{
    public class ConverterRegistry
        : IConverterRegistry
    {
        private readonly Dictionary<Type, IEventConverter<IDomainEvent>> _registry =
            new Dictionary<Type, IEventConverter<IDomainEvent>>();

        public void Register<TEventFrom, TEventTo>(AbstractEventConverter<TEventFrom, TEventTo> eventConverter)
            where TEventFrom : IDomainEvent
            where TEventTo : IDomainEvent
        {
            _registry.Add(typeof(TEventFrom), eventConverter);
        }

        public IDomainEvent Convert<TEventFrom>(TEventFrom eventFrom)
            where TEventFrom : IDomainEvent
        {
            if (!_registry.TryGetValue(eventFrom.GetType(), out var eventConverter))
                return eventFrom;
            var convertedEvent = eventConverter.Convert(eventFrom);
            return Convert((dynamic) convertedEvent);
        }
    }
}