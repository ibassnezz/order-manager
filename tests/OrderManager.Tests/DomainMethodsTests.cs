using System;
using System.Collections.Generic;
using System.Linq;
using OrderManager.Domain;
using OrderManager.Domain.Aggregate;
using OrderManager.Events;
using Xunit;

namespace OrderManager.Tests
{
    public class DomainMethodsTests
    {
        [Theory]
        [InlineData("", "AAAA")]
        [InlineData("AAAA", "")]
        [InlineData("AAAA", "BBBB")]
        public void SetRequisitesTests(string billNumber, string paymentType)
        {
            var refund = new Order("00-00", new EventStream<OrderSnapshotObject>(0, null, new Queue<IDomainEvent>()));
            refund.SetRequisites(billNumber, paymentType);
            refund.SetRequisites(billNumber, paymentType);
            Assert.Single(refund.GetPendingEvents());

            Assert.Throws<ArgumentException>(() => refund.SetRequisites(paymentType, billNumber));
        }

        [Fact]
        public void AddWireTests()
        {
            var refund = new Order("00-00", new EventStream<OrderSnapshotObject>(0, null, new Queue<IDomainEvent>()));
            refund.AddComponent(1, 100, new Product[0]);
            refund.AddComponent(2, 200, new [] { new Product("1", 1, 10) });
            refund.AddComponent(3, 200, new [] { new Product("1", 2, 1000000) });

            Assert.Single(refund.Products);
            Assert.Equal(3, refund.Products.First().Quantity);
            Assert.Equal(10, refund.Products.First().Price);

            Assert.Throws<ArgumentException>(() => refund.AddComponent(3, 200, new[] { new Product("1", -2, 10) }));
            Assert.Throws<ArgumentException>(() => refund.AddComponent(1, 200, new Product[0]));
        }


        [Fact]
        public void ReturnDoneAllTests()
        {
            var refund = new Order("00-00", new EventStream<OrderSnapshotObject>(0, null, new Queue<IDomainEvent>()));
            refund.AddComponent(1, 100, new Product[0]);
            refund.AddComponent(2, 200, new[] { new Product("1", 1, 10) });

            refund.SetAllProcessed();
            Assert.All(refund.Components, wire => Assert.True(wire.IsHandled));
            Assert.Empty(refund.Products);
            Assert.Equal(decimal.Zero, refund.Amount);
        }


        [Fact]
        public void ReturnDoneTests()
        {
            var refund = new Order("00-00", new EventStream<OrderSnapshotObject>(0, null, new Queue<IDomainEvent>()));
            refund.AddComponent(1, 100, new Product[0]);
            refund.AddComponent(2, 200, new[] { new Product("1", 1, 10) });

            refund.ProcessDone(new long[] {1, 2}, 300, new [] { new Events.EventStack.V2.ItemIdQuantityPair("1", 0) });
            Assert.Throws<ArgumentException>(() => refund.ProcessDone(new long[] { 1 }, 100000, new[] { new Events.EventStack.V2.ItemIdQuantityPair("1", 0) }));
            Assert.Throws<ArgumentException>(() => refund.ProcessDone(new long[] { 1 }, 100, new[] { new Events.EventStack.V2.ItemIdQuantityPair("123", 0) }));
            Assert.All(refund.Components, wire => Assert.True(wire.IsHandled));
            Assert.Empty(refund.Products);
            Assert.Equal(decimal.Zero, refund.Amount);
            Assert.Throws<ArgumentException>(() => refund.ProcessDone(new long[] { 1 }, 10, new[] { new Events.EventStack.V2.ItemIdQuantityPair("1", 0) }));
            Assert.Throws<ArgumentException>(() => refund.ProcessDone(new long[] { 3 }, 10, new[] { new Events.EventStack.V2.ItemIdQuantityPair("1", 0) }));
        }

    }
}
