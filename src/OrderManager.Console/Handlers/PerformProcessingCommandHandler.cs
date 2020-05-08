using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderManager.Console.Application;
using OrderManager.Domain.Aggregate;
using OrderManager.Domain.Storage;
using OrderManager.Integration;

namespace OrderManager.Console.Handlers
{
    public class PerformProcessingCommandHandler : IRequestHandler<PerformProcessingCommand>
    {
        private readonly ILogger<PerformProcessingCommandHandler> _logger;
        private readonly IRepository<Order, string> _eventRepository;
        private readonly IProcessingProviderService _refunderProviderService;
        private readonly ScheduleService _scheduleService;

        public PerformProcessingCommandHandler(
            ILogger<PerformProcessingCommandHandler> logger, 
            IRepository<Order, string> eventRepository,
            IProcessingProviderService refunderProviderService,
            ScheduleService scheduleService)
        {
            _logger = logger;
            _eventRepository = eventRepository;
            _refunderProviderService = refunderProviderService;
            _scheduleService = scheduleService;
        }


        public async Task<Unit> Handle(PerformProcessingCommand request, CancellationToken cancellationToken)
        {
            var refund = await _eventRepository.GetAsync(request.OrderNumber, cancellationToken);

            if (!refund.HasComponentsToProcess)
            {
                _logger.LogWarning($"Order {request.OrderNumber} has no wires to cancel");
                return Unit.Value;
            }
            
            var refundResult = ProcessedResult.Failed;

            try
            {
                refundResult = await _refunderProviderService.TryToProcess(refund, cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error requesting refund");
            }

            if (refundResult == ProcessedResult.Failed)
            {
                await _scheduleService.Schedule(request.OrderNumber, cancellationToken);
                return Unit.Value;
            }

            try
            {
                refund.SetAllProcessed();
            }
            catch (ArgumentException e)
            {
                _logger.LogError(e, "Cannot SetAllProcessed");
                return Unit.Value;
            }
            

            await _eventRepository.UpdateAsync(refund, cancellationToken);

            return Unit.Value;
        }
    }
}
