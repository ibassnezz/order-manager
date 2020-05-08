using System.Threading;
using System.Threading.Tasks;
using OrderManager.Domain.Aggregate;

namespace OrderManager.Domain.Storage
{
    public interface IRepository<TInstance, TKey> where TInstance : IAggregateRoot
    {
        Task<TInstance> GetAsync(TKey key, CancellationToken cancellationToken);

        Task UpdateAsync(TInstance instance, CancellationToken cancellationToken);
    }
}
