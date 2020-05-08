using System.Collections.Generic;
using OrderManager.Events;
using OrderManager.Events.Snapshot;

namespace OrderManager.Domain
{
    public class EventStream<TSnapshotData> where TSnapshotData : ISnapshotData
    {
        public EventStream(int mutation, Snapshot<TSnapshotData> snapshot, Queue<IDomainEvent> domainEvents)
        {
            Mutation = mutation;
            Snapshot = snapshot;
            DomainEvents = domainEvents;
        }

        public int Mutation { get; }
        public Snapshot<TSnapshotData> Snapshot { get; }

        public Queue<IDomainEvent> DomainEvents { get; }
    }
}
