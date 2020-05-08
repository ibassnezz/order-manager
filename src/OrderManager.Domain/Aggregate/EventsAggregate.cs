using System.Collections.Generic;
using OrderManager.Events;
using OrderManager.Events.Snapshot;

namespace OrderManager.Domain.Aggregate
{
    public abstract class EventsAggregate<TSnapshotData> :
        IAggregateRoot, 
        IAggregateSnapshot<TSnapshotData> where TSnapshotData : ISnapshotData
    {
        private const int SnapshotPeriod = 10;

        public Snapshot<TSnapshotData> Snapshot { get; private set; }

        public int PersistentVersion { get; private set; }

        protected EventsAggregate(EventStream<TSnapshotData> stream)
        {
            PersistentVersion = stream.Mutation;
            if (stream.Snapshot != null)
            {
                Restore(stream.Snapshot.Payload);
            }
            Replay(stream.DomainEvents);
        }

        private readonly Queue<IDomainEvent> _domainEvents = new Queue<IDomainEvent>();

        public Queue<IDomainEvent> GetPendingEvents()
        {
            return new Queue<IDomainEvent>(_domainEvents.ToArray());
        }
        protected void Update(IDomainEvent domainEvent)
        {
            Mutate(domainEvent);
            AppendEvent(domainEvent);
        }
        
        public void Replay(Queue<IDomainEvent> domainEvents)
        {
            while (domainEvents.TryDequeue(out var domainEvent))
            {
                Mutate(domainEvent);
            }
        }

        protected void Mutate<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent
        {
            ((dynamic)this).When((dynamic)domainEvent);
        }

        private void AppendEvent(IDomainEvent @event)
        {
            _domainEvents.Enqueue(@event);
            if (ShouldMakeSnapshot())
            {
                Snapshot = new Snapshot<TSnapshotData>(CurrentVersion, Catch());
            }
        }

        /// <summary>
        /// Clean non persistent event
        /// </summary>
        public void Flush()
        {
            PersistentVersion += _domainEvents.Count;
            _domainEvents.Clear();
            Snapshot = null;
        }

        private int CurrentVersion => PersistentVersion + _domainEvents.Count;

        private bool ShouldMakeSnapshot() => CurrentVersion % SnapshotPeriod == 0;

        public abstract void Restore(TSnapshotData snapshot);

        public abstract TSnapshotData Catch();
    }
}
