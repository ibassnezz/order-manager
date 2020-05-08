using System.Threading;
using System.Threading.Tasks;

namespace OrderManager.Domain.Storage
{
    public interface IEventDataLayer
    {
        Task AppendRecordsAsync(string orderNumber, RawDataContainer data, CancellationToken cancellationToken);
        Task<RawDataContainer> ReadRecordsAsync(string streamName, CancellationToken cancellationToken);
    }
}