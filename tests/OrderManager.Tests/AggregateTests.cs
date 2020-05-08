using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using OrderManager.Domain;
using OrderManager.Domain.Aggregate;
using OrderManager.Events;
using OrderManager.Events.EventStack;
using OrderManager.Events.EventStack.V1;
using V2 = OrderManager.Events.EventStack.V2;
using Xunit;

namespace OrderManager.Tests
{
    public class AggregateTests
    {
        
        private const string OrderNumber = "0000-000";
        private const string CheckNumber = "23456789";
        private const string Description = "Bank card";

        [Fact]
        public void CreateAggregate()
        {
            var queue = new Queue<IDomainEvent>();
            var aggregate = new Order(OrderNumber, new EventStream<OrderSnapshotObject>(queue.Count, null, queue));
            var events = aggregate.GetPendingEvents();
            Assert.True(events.IsNullOrEmpty());
            Assert.Equal(OrderNumber, aggregate.OrderNumber);
            Assert.Equal(decimal.Zero, aggregate.Amount);
            Assert.Empty(aggregate.Components);
            Assert.Empty(aggregate.Products);
            Assert.Null(aggregate.OrderDescription);
            Assert.Null(aggregate.CheckNumber);
        }

        [Fact]
        public void RestoreAggregate()
        {
            var componentAddedEvents = CreateWireAddedEvents();
                
            var queue = new Queue<IDomainEvent>();
            Array.ForEach(componentAddedEvents, wire => queue.Enqueue(wire));
            queue.Enqueue(new RequisitesEvent(Description, CheckNumber));
            
            var aggregate = new Order(OrderNumber, new EventStream<OrderSnapshotObject>(queue.Count, null, queue));
            
            var events = aggregate.GetPendingEvents();
            Assert.True(events.IsNullOrEmpty());

            Assert.Equal(OrderNumber, aggregate.OrderNumber);
            Assert.Equal(componentAddedEvents.Sum(x => x.Amount), aggregate.Amount);
            Assert.NotEmpty(aggregate.Components);
            Assert.Equal(componentAddedEvents.Length, aggregate.Components.Count);
            Assert.Equal(componentAddedEvents.Sum(x => x.Products.Length), aggregate.Products.Count);
            
            Assert.True(aggregate.Components.All(x => componentAddedEvents.Any(w => w.ComponentId ==  x.Id)));
            Assert.True(aggregate.Products.All(x => componentAddedEvents.SelectMany(w => w.Products).Any(p => p.ItemId == x.ItemId)));
            
            Assert.Equal(Description, aggregate.OrderDescription);
            Assert.Equal(CheckNumber, aggregate.CheckNumber);

        }

        private static ComponentAddedEvent[] CreateWireAddedEvents()
        {
            var wireEvents = new ComponentAddedEvent[]
            {
                new ComponentAddedEvent(
                    componentId: 1,
                    amount: 100,
                    products: new ProductSourceObject[] { new ProductSourceObject { ItemId = "1", Price = 10, Quantity = 1 } }),
                new ComponentAddedEvent(
                    componentId: 2,
                    amount: 200,
                    products: new ProductSourceObject[] { new ProductSourceObject { ItemId = "2", Price = 20, Quantity = 1 } })
            };
            return wireEvents;
        }


        [Fact]
        public void FlipFlopAggregate()
        {
            var queue = new Queue<IDomainEvent>();
            
            var aggregate = new Order(OrderNumber, new EventStream<OrderSnapshotObject>(queue.Count, null, queue));
            aggregate.SetRequisites(CheckNumber, Description);
            var wireBeforeEvents = CreateWireAddedEvents();
            Array.ForEach(
                wireBeforeEvents,
                wire => aggregate.AddComponent(
                    wire.ComponentId,
                    wire.Amount,
                    wire.Products.Select(x => new Product(x.ItemId, x.Quantity, x.Price)).ToArray()));
            
            aggregate.ProcessDone(new long[] { 2 }, processedAmount: 200, new [] { new V2.ItemIdQuantityPair("2", 0)});
            
            var events = aggregate.GetPendingEvents();
            Assert.Equal(OrderNumber, aggregate.OrderNumber);

            var flipped = new Order(OrderNumber, new EventStream<OrderSnapshotObject>(events.Count, null, events));

            Assert.Equal(aggregate.Amount, flipped.Amount);
            Assert.Equal(aggregate.OrderDescription, flipped.OrderDescription);
            Assert.Equal(aggregate.OrderNumber, flipped.OrderNumber);
            Assert.Equal(aggregate.CheckNumber, flipped.CheckNumber);
            Assert.Contains(aggregate.Components, x => x.IsHandled);
            Assert.Contains(aggregate.Components, x => !x.IsHandled);

            var products = from p1 in aggregate.Products
                join p2 in flipped.Products on new { p1.ItemId, p1.Price, p1.Quantity }
                    equals new { p2.ItemId, p2.Price, p2.Quantity }
                select new {p1.ItemId};

            var wires = from w1 in aggregate.Components
                join w2 in flipped.Components on new { w1.Id, w1.IsHandled } equals new { w2.Id, w2.IsHandled }
                select new { w1.Id };

            Assert.Equal(aggregate.Products.Count, products.Count());
            Assert.Equal(aggregate.Components.Count, wires.Count());

        }
    }
}
