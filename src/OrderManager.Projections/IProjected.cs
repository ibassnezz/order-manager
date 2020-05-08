using System.Threading;
using System.Threading.Tasks;
using OrderManager.Events;

namespace OrderManager.Projections
{
    internal interface IProjected<TEvent> where TEvent: IDomainEvent 
    { 
        Task Projected(string key, TEvent domainEvent, CancellationToken cancellationToken);
    }
}
