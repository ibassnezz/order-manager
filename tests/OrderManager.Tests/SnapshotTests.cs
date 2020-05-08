using System.Collections.Generic;
using System.Linq;
using OrderManager.Domain;
using OrderManager.Domain.Aggregate;
using OrderManager.Events;
using OrderManager.Events.EventStack;
using OrderManager.Events.EventStack.V1;
using OrderManager.Events.Snapshot;
using Xunit;

namespace OrderManager.Tests
{
    public class SnapshotTests
    {
        [Fact]
        public void RestoreSnapshot()
        {
            var sum = 100M;
            var checkNumber = "checkNumber";
            var orderDescription = "orderNumber";
            var components = new Component[] {new Component(1)};
            var products = new Product[] {new Product("2", 1, 1), };

            var snapshot =
                new Snapshot<OrderSnapshotObject>(10, new OrderSnapshotObject(checkNumber, orderDescription, components,
                    products, sum)){};

            var aggregate = new Order(string.Empty, new EventStream<OrderSnapshotObject>(0, snapshot, new Queue<IDomainEvent>()));
            Assert.Equal(aggregate.Amount, sum);
            Assert.Equal(aggregate.CheckNumber, checkNumber);
            Assert.Equal(aggregate.OrderDescription, orderDescription);
            Assert.Collection(aggregate.Products, p =>
            {
                Assert.Equal(p.ItemId, products[0].ItemId);
                Assert.Equal(p.Price, products[0].Price);
                Assert.Equal(p.Quantity, products[0].Quantity);
            });
            Assert.Collection(aggregate.Components, c =>
            {
                Assert.Equal(c.Id, components[0].Id);
                Assert.Equal(c.IsHandled, components[0].IsHandled);
            });

        }

        [Fact]
        public void CatchSnapshot()
        {
            var aggregate = new Order(string.Empty, new EventStream<OrderSnapshotObject>(0, null, new Queue<IDomainEvent>()));
            for (var i = 0; i < 10; i++)
            {
                aggregate.AddComponent(i, 10, new[] { new Product("1", 1, 1)});
            }
            Assert.NotNull(aggregate.Snapshot);
        }
    }
}
