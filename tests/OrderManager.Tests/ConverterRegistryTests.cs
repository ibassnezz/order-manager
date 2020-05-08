using System;
using OrderManager.Events;
using Xunit;
using ConverterRegistry = OrderManager.Domain.Storage.ConverterRegistry;

namespace OrderManager.Tests
{
    public class ConverterRegistryTests
    {
        [Fact]
        public void ConverterWithNoSuitableEventsTest()
        {
            var registry = new ConverterRegistry();
            var guid = Guid.NewGuid();
            var eventToConvert = new EventV1 { Id = guid };
            var result = registry.Convert(eventToConvert);
            Assert.True(result is EventV1);
            Assert.Equal(guid, ((EventV1)result).Id);
            Assert.Same(eventToConvert, result);
        }

        [Fact]
        public void OneLevelConvertWithRegisteredEventTest()
        {
            var registry = new ConverterRegistry();
            var message = "test";
            var guid = Guid.NewGuid();
            var eventToConvert = new EventV1 { Id = guid };
            
            registry.Register(new EventConverterV1V2(message));
            
            var result = registry.Convert(eventToConvert);

            Assert.NotSame(eventToConvert, result);
            Assert.True(result is EventV2);
            Assert.Equal(guid, ((EventV2)result).NewGuid);
            Assert.Equal(message, ((EventV2)result).Message);
        }


        [Fact]
        public void TwoLevelConvertWithRegisteredEventTest()
        {
            var registry = new ConverterRegistry();
            var message = "test";
            var number = 1;
            var guid = Guid.NewGuid();
            var eventToConvert = new EventV1 { Id = guid };

            registry.Register(new EventConverterV1V2(message));
            registry.Register(new EventConverterV2V3(number));

            var result = registry.Convert(eventToConvert);

            Assert.NotSame(eventToConvert, result);
            Assert.True(result is EventV3);
            Assert.Equal(guid, ((EventV3)result).NewNewGuid);
            Assert.Equal(message, ((EventV3)result).NewMessage);
        }

    }

    public class EventV1 : IDomainEvent
    {
        public Guid Id { get; set; }
    }

    public class EventV2 : IDomainEvent
    {
        public Guid NewGuid { get; set; }

        public string Message { get; set; }
    }

    public class EventV3 : IDomainEvent
    {
        public Guid NewNewGuid { get; set; }

        public string NewMessage { get; set; }

        public int Number { get; set; }
    }


    internal class EventConverterV2V3 : AbstractEventConverter<EventV2, EventV3>
    {
        private readonly int _defaultNumber;

        public EventConverterV2V3(int defaultNumber)
        {
            _defaultNumber = defaultNumber;
        }

        public override EventV3 Convert(EventV2 eventFrom)
        {
            var result = new EventV3 { NewMessage = eventFrom.Message, NewNewGuid = eventFrom.NewGuid, Number = _defaultNumber };
            return result;
        }
    }


    internal class EventConverterV1V2 : AbstractEventConverter<EventV1, EventV2>
    {
        private string _message;
        public EventConverterV1V2(string message)
        {
            _message = message;
        }

        public override EventV2 Convert(EventV1 eventFrom)
        {
            var newEvent = new EventV2();
            newEvent.NewGuid = eventFrom.Id;
            newEvent.Message = _message;
            return newEvent;
        }
    }
}
