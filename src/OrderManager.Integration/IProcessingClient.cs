using System.Threading;
using System.Threading.Tasks;

namespace OrderManager.Integration
{
    public interface IProcessingClient
    {
        Task<ApiResponse> Execute(OrderProcessingDto cancellationDto, CancellationToken cancellationToken);
    }
}
