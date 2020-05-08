using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Niazza.KafkaMessaging.Consumer;
using OrderManager.Console.Handlers;

namespace OrderManager.Console.Bus
{
    public class SendToProcessingMessageHandler : AbstractMessageHandler<SendToProcessingMessage>
    {
        private readonly IMediator _mediator;

        public SendToProcessingMessageHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        protected override async Task<ExecutionResult> HandleAsync(SendToProcessingMessage message, CancellationToken cancellationToken)
        {
            await _mediator.Send(new PerformProcessingCommand(message.OrderNumber), cancellationToken);
            return ExecutionResult.Acknowledged;
        } 
        

    }
}