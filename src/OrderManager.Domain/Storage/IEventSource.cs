using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OrderManager.Events;
using OrderManager.Events.Snapshot;

namespace OrderManager.Domain.Storage
{
    public interface IEventSource<TSnapshotData> where TSnapshotData : ISnapshotData
    {
        Task<EventStream<TSnapshotData>> GetAsync(string orderNumber, CancellationToken cancellationToken);

        Task AppendAsync(string orderNumber, int persistentVersion, Snapshot<TSnapshotData> snapshot, Queue<IDomainEvent> domainEvents, CancellationToken cancellationToken);
    }
}