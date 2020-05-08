using System.Threading;
using System.Threading.Tasks;
using OrderManager.Domain.Aggregate;

namespace OrderManager.Integration
{
    public interface IProcessingProviderService
    {
        Task<ProcessedResult> TryToProcess(Order order, CancellationToken cancellationToken);
    }
}