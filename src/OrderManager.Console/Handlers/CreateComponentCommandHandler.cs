using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderManager.Console.Application;
using OrderManager.Domain;
using OrderManager.Domain.Aggregate;
using OrderManager.Domain.Storage;

namespace OrderManager.Console.Handlers
{
    public class CreateComponentCommandHandler : IRequestHandler<CreateOrderComponentCommand>
    {
        private readonly ILogger<CreateComponentCommandHandler> _logger;
        private readonly IRepository<Order, string> _repository;
        private readonly ScheduleService _scheduleService;

        public CreateComponentCommandHandler(ILogger<CreateComponentCommandHandler> logger,  IRepository<Order, string> repository, ScheduleService scheduleService)
        {
            _logger = logger;
            _repository = repository;
            _scheduleService = scheduleService;
        }
        
        public async Task<Unit> Handle(CreateOrderComponentCommand request, CancellationToken cancellationToken)
        {
            var refund = await _repository.GetAsync(request.OrderNumber, cancellationToken);
           
            refund.SetRequisites(request.BillNumber, request.PaymentsType);

            try
            {
                refund.AddComponent(
                    request.ComponentId,
                    request.Amount,
                    request.Products.Select(p => new Product(p.ItemId.ToString(), p.Quantity, p.Price)).ToArray());

                await _scheduleService.Schedule(request.OrderNumber, cancellationToken);
                await _repository.UpdateAsync(refund, cancellationToken);
            }
            catch (ArgumentException e)
            {
                _logger.LogError(e, "Incorrect data");
            }
           
            return Unit.Value;
        }
    }
}
