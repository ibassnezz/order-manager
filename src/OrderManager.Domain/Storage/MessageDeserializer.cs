using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using OrderManager.Events;

namespace OrderManager.Domain.Storage
{
    public class MessageDeserializer: IMessageDeserializer
    {
        private readonly IDictionary<string, Type> _types = new ConcurrentDictionary<string, Type>();

        public MessageDeserializer(Type[] eventTypes)
        {
            Array.ForEach(eventTypes,
                type =>
                {
                    if (!(type.GetCustomAttribute(typeof(EventIdAttribute)) is EventIdAttribute attribute))
                    {
                        throw new Exception($"The event has no {nameof(EventIdAttribute)}");
                    }
                    _types.Add(attribute.EventName, type);
                });
        }
        
        public IDomainEvent DeserializeEvent(string eventName, JObject jObject)
        {
            if (!_types.TryGetValue(eventName, out var type))
            {
                throw new ArgumentException($"Cannot find event deserializer for {eventName}");
            }
            return (IDomainEvent)jObject.ToObject(type);
        }

        public string GetEventName(IDomainEvent type)
        {
            return _types.First(x => x.Value == type.GetType()).Key;
        }
    }

    public static class EventConverterExtensions
    {
        public static string GetName(this IDomainEvent converter)
        {
            if (!(converter.GetType().GetCustomAttribute(typeof(EventIdAttribute)) is EventIdAttribute attribute))
            {
                throw new Exception($"The event has no {nameof(EventIdAttribute)}");
            }
            return attribute.EventName;
        }
    }
}
