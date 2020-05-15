using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OrderManager.Events;
using OrderManager.Events.Snapshot;

namespace OrderManager.Domain.Storage
{
    public class EventStore<TSnapshotData> : IEventSource<TSnapshotData> where TSnapshotData : ISnapshotData
    {
        private readonly IEventDataLayer _eventRepository;
        private readonly IConverterRegistry _converterRegistry;
        private readonly IMessageDeserializer _messageDeserializer;
        private readonly IMediator _mediator;

        public EventStore(IEventDataLayer eventRepository, IConverterRegistry converterRegistry, IMessageDeserializer messageDeserializer, IMediator mediator)
        {
            _eventRepository = eventRepository;
            _converterRegistry = converterRegistry;
            _messageDeserializer = messageDeserializer;
            _mediator = mediator;
        }

        public async Task<EventStream<TSnapshotData>> GetAsync(string orderNumber, CancellationToken cancellationToken)
        {
            var container = await _eventRepository.ReadRecordsAsync(orderNumber, cancellationToken);
            var events = container.RawEvents;
            var query = new Queue<IDomainEvent>();

            foreach (var dataWithVersion in events)
            {
                var domainEvent = _messageDeserializer.DeserializeEvent(dataWithVersion.Type, dataWithVersion.Data);
                var convertedEvent = _converterRegistry.Convert(domainEvent);
                query.Enqueue(convertedEvent);
            }

            var snapshot = container.RawSnapshot is null? default : new Snapshot<TSnapshotData>(container.RawSnapshot.LastVersion, container.RawSnapshot.Raw.ToObject<TSnapshotData>());

            return new EventStream<TSnapshotData> (events.Any() ? events.Max(x => x.Version) : snapshot?.LastVersion ?? 0, snapshot, query);
        }

        public async Task AppendAsync(string orderNumber, int persistentVersion, Snapshot<TSnapshotData> snapshot, Queue<IDomainEvent> domainEvents, CancellationToken cancellationToken)
        {
            if (!domainEvents.Any())
            {
                return;
            }

            var persistIncrement = persistentVersion;
            var dataQueue = new Queue<DataWithVersion>();
            var publications = domainEvents.ToArray();

            while (domainEvents.TryDequeue(out var domainEvent))
            {
                persistIncrement++;
                dataQueue.Enqueue(new DataWithVersion(persistIncrement, JObject.FromObject(domainEvent), _messageDeserializer.GetEventName(domainEvent)));
            }

            var rawSnapshot = snapshot is null
                ? default
                : new RawSnapshotData(snapshot.LastVersion, JObject.FromObject(snapshot.Payload));

            await _eventRepository.AppendRecordsAsync(orderNumber, new RawDataContainer(rawSnapshot, dataQueue), cancellationToken);

            var tasks = publications.Select(p =>
                _mediator.Publish(new EventNotification<string, IDomainEvent>(orderNumber, p), cancellationToken));

            await Task.WhenAll(tasks);
        }

    }
}