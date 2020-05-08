using Newtonsoft.Json.Linq;
using OrderManager.Events;

namespace OrderManager.Domain.Storage
{
    public interface IMessageDeserializer
    {
        IDomainEvent DeserializeEvent(string eventName, JObject jObject);
        string GetEventName(IDomainEvent type);
    }
}
