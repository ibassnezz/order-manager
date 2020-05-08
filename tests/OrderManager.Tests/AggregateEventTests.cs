using System.Collections.Generic;
using System.Linq;
using OrderManager.Domain;
using OrderManager.Domain.Aggregate;
using OrderManager.Events;
using OrderManager.Events.EventStack.V1;
using V2 = OrderManager.Events.EventStack.V2;
using Xunit;

namespace OrderManager.Tests
{
    public class AggregateEventTests
    {
        [Fact]
        public void ProcessedEventTests()
        {
            var sum = 100M;
            var componentId = 1;
            var product = "1";

            var aggregate = new Order(string.Empty, new EventStream<OrderSnapshotObject>(0, null, new Queue<IDomainEvent>()));
            aggregate.AddComponent(componentId, sum, new [] {new Product(product, 1, 1) });
            aggregate.ProcessDone(new long[] {componentId}, sum, new []{ new V2.ItemIdQuantityPair(product, 0) });

            var processedEvent = (V2.ProcessedEvent)aggregate.GetPendingEvents().First(x => x is V2.ProcessedEvent);
            
            Assert.Equal(product, processedEvent.ProductValues[0].ItemId);
            Assert.Equal(componentId, processedEvent.ComponentIds[0]);
            Assert.Equal(sum, processedEvent.Amount);

        }
        
        [Fact]
        public void RequisitesEventTests()
        {
            var billNumber = "123";
            var paymentType = "BankCard"; 

            var aggregate = new Order(string.Empty, new EventStream<OrderSnapshotObject>(0, null, new Queue<IDomainEvent>()));
            aggregate.SetRequisites(billNumber, paymentType);
            
            var requisitesEvent = (RequisitesEvent)aggregate.GetPendingEvents().First(x => x is RequisitesEvent);

            Assert.Equal(paymentType, requisitesEvent.OrderDescription);
            Assert.Equal(billNumber, requisitesEvent.CheckNumber);

        }

        [Fact]
        public void ComponentAddedEvent()
        {
            var sum = 100M;
            var componentId = 1;
            var productId = "1";
            var products = new [] {new Product(productId, 1, sum), };

            var aggregate = new Order(string.Empty, new EventStream<OrderSnapshotObject>(0, null, new Queue<IDomainEvent>()));
            aggregate.AddComponent(componentId, sum, products);

            var wireEvent = (ComponentAddedEvent)aggregate.GetPendingEvents().First(x => x is ComponentAddedEvent);

            Assert.Equal(componentId, wireEvent.ComponentId);
            Assert.Equal(sum, wireEvent.Amount);
            var product = wireEvent.Products.First();
            Assert.Equal(sum, product.Price);
            Assert.Equal(productId, product.ItemId);
            Assert.Equal(1, product.Quantity);
            
        }
        
    }
}
