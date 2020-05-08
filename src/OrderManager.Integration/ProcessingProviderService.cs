using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OrderManager.Domain.Aggregate;

namespace OrderManager.Integration
{
    public class ProcessingProviderService
        : IProcessingProviderService
    {
        private readonly IProcessingClient _client;
        private readonly bool _isTest;

        public ProcessingProviderService(IProcessingClient client, IOptions<ProcessingConfigurations> options)
        {
            _client = client;
            _isTest = options.Value.IsTest;
        }

        public async Task<ProcessedResult> TryToProcess(Order order, CancellationToken cancellationToken)
        {
            if (_isTest)
            {
                return ProcessedResult.Executed;
            }

            var cancellationDto = new OrderProcessingDto
            {
                Products = order.Products.Select(p => new ProductDto(p.ItemId, p.Price, p.Quantity)).ToArray(),
                Description = order.OrderDescription,
                ComponentIds = order.Components.Select(x => x.Id).ToArray(),
                CheckNumber = order.CheckNumber,
                OrderNumber = order.OrderNumber,
                Amount = order.Amount
            };

            var result = await _client.Execute(cancellationDto, cancellationToken);

            return result.IsSuccess
                ? ProcessedResult.Executed
                : ProcessedResult.Failed;
        }

    }
}
