using System;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OrderManager.Domain.Storage;
using OrderManager.Events;
using OrderManager.Events.EventStack.V1;
using OrderManager.Infrastructure.Repository;
using V2 = OrderManager.Events.EventStack.V2;

namespace OrderManager.Projections
{
    public class ComponentStatusHandler : INotificationHandler<EventNotification<string, IDomainEvent>>,
        IProjected<ComponentAddedEvent>,
        IProjected<V2.ProcessedEvent>,
        IProjected<IDomainEvent>
    {
        private readonly IOrderComponentRepository _componentRepository;

        public ComponentStatusHandler(IOrderComponentRepository componentRepository)
        {
            _componentRepository = componentRepository;
        }

        public async Task Handle(EventNotification<string, IDomainEvent> notification, CancellationToken cancellationToken)
        {
            await Projected(notification.Key, (dynamic)notification.DomainEvent, cancellationToken);
        }

        public Task Projected(string key, ComponentAddedEvent domainEvent, CancellationToken cancellationToken)
        {
            return _componentRepository.CreateAsync(key, domainEvent.ComponentId, domainEvent.Amount, cancellationToken);
        }

        public async Task Projected(string key, V2.ProcessedEvent domainEvent, CancellationToken cancellationToken)
        {
            var processed = domainEvent.ComponentIds.Select(cid => _componentRepository.SetProcessedAsync(cid, cancellationToken));
            await Task.WhenAll(processed);
        }

        public Task Projected(string key, IDomainEvent domainEvent, CancellationToken cancellationToken) => Task.CompletedTask;
        
    }
}
