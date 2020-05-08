using System;

namespace OrderManager.Events
{
    public class EventIdAttribute: Attribute
    {
        public string EventName { get; }

        public EventIdAttribute(string eventName)
        {
            EventName = eventName;
        }
    }
}
