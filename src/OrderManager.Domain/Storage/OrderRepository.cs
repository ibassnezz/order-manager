using System.Threading;
using System.Threading.Tasks;
using OrderManager.Domain.Aggregate;

namespace OrderManager.Domain.Storage
{
    public class OrderRepository : IRepository<Order, string>
    {
        private readonly IEventSource<OrderSnapshotObject> _eventSource;

        public OrderRepository(IEventSource<OrderSnapshotObject> eventSource)
        {
            _eventSource = eventSource;
        }

        public async Task<Order> GetAsync(string key, CancellationToken cancellationToken)
        {
            var stream = await _eventSource.GetAsync(key, cancellationToken);
            var refund = new Order(key, stream);
            return refund;
        }

        public async Task UpdateAsync(Order instance, CancellationToken cancellationToken)
        {
            await _eventSource.AppendAsync(instance.OrderNumber, instance.PersistentVersion, instance.Snapshot, instance.GetPendingEvents(), cancellationToken);
            instance.Flush();
        }
    }
}
