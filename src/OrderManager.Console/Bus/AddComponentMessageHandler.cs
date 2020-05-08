using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Niazza.KafkaMessaging.Consumer;
using OrderManager.Console.Handlers;
using OrderManager.Infrastructure.Repository;

namespace OrderManager.Console.Bus
{
    public class AddComponentMessageHandler: AbstractMessageHandler<AddComponentMessage>
    {
        private readonly IMediator _mediator;
        private readonly IOrderComponentRepository _componentRepository;

        public AddComponentMessageHandler(IMediator mediator, IOrderComponentRepository componentRepository)
        {
            _mediator = mediator;
            _componentRepository = componentRepository;
        }

        protected override async Task<ExecutionResult> HandleAsync(AddComponentMessage message, CancellationToken cancellationToken)
        {
            var component = await _componentRepository.GetAsync(message.Id, cancellationToken);

            if (component.Any()) return ExecutionResult.PassOut;

            await _mediator.Send(
                new CreateOrderComponentCommand(
                    message.Id,
                    message.OrderNumber,
                    message.CheckNumber,
                    message.OrderDescription,
                    message.Products.Select(x => new Handlers.ComponentProduct(x.ItemId, x.Quantity, x.Price)).ToArray(),
                    message.Amount,
                    message.IsDelivery),
                cancellationToken);

            return ExecutionResult.Acknowledged;
        }
    }
}
