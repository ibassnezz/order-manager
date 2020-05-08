using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OrderManager.Infrastructure.Database;

namespace OrderManager.Infrastructure.Repository
{
    public interface IOrderComponentRepository
    {
        Task CreateAsync(string orderNumber, long componentId, decimal amount, CancellationToken cancellationToken);
        Task SetProcessedAsync(long componentId, CancellationToken cancellationToken);
        Task<IEnumerable<OrderComponent>> GetAsync(long componentId, CancellationToken cancellationToken);
    }
}