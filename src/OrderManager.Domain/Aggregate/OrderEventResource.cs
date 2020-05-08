using System;
using System.Collections.Generic;
using System.Linq;
using V1 = OrderManager.Events.EventStack.V1;
using V2 = OrderManager.Events.EventStack.V2;

namespace OrderManager.Domain.Aggregate
{
    public partial class Order: EventsAggregate<OrderSnapshotObject>, 
        IWhen<V1.ComponentAddedEvent>, 
        IWhen<V2.ProcessedEvent>, 
        IWhen<V1.RequisitesEvent>
    {
        
        public Order(string orderNumber, EventStream<OrderSnapshotObject> stream): base(stream)
        {
            OrderNumber = orderNumber;
        }
        
        public void When(V1.ComponentAddedEvent domainEvent)
        {
            Amount += domainEvent.Amount;
            _components.Add(new Component(domainEvent.ComponentId));

            foreach (var product in domainEvent.Products)
            {
                var domainProduct = _products.FirstOrDefault(x => x.ItemId == product.ItemId);

                if (domainProduct is null)
                {
                    _products.Add(new Product(product.ItemId, product.Quantity, product.Price));
                }
                else
                {
                    domainProduct.AddQuantity(product.Quantity);
                }
            }
        }

        public void When(V2.ProcessedEvent domainEvent)
        {
            foreach (var item in domainEvent.ProductValues)
            {
                var product = _products.First(x => x.ItemId.Equals(item.ItemId));
                if (product.Quantity == item.Quantity || item.ShouldProcessAll)
                {
                    _products.Remove(product);
                }
                product.SubtractQuantity(item.Quantity);
            }

            Array.ForEach(_components.Where(x => domainEvent.ComponentIds.Contains(x.Id)).ToArray(),
                wire => wire.Handled()); 
            Amount -= domainEvent.Amount;

        }

        public void When(V1.RequisitesEvent domainEvent)
        {
            CheckNumber = domainEvent.CheckNumber;
            OrderDescription = domainEvent.OrderDescription;
        }

        public override void Restore(OrderSnapshotObject snapshot)
        {
            CheckNumber = snapshot.CheckNumber;
            Amount = snapshot.Amount;
            _products.Clear();
            _products.AddRange(snapshot.Products);
            _components.Clear();
            _components.AddRange(snapshot.Components);
            OrderDescription = snapshot.OrderDescription;
        }

        public override OrderSnapshotObject Catch()
        {
            return new OrderSnapshotObject(CheckNumber, OrderDescription, Components.ToArray(), Products.ToArray(), Amount);
        }
    }
}
