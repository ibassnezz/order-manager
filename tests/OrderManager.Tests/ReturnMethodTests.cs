using System.Collections.Generic;
using OrderManager.Domain;
using OrderManager.Domain.Aggregate;
using OrderManager.Events;
using OrderManager.Events.EventStack;
using OrderManager.Events.EventStack.V1;
using V2=OrderManager.Events.EventStack.V2;
using Xunit;

namespace OrderManager.Tests
{
    public class ReturnMethodTests
    {
        [Fact]
        public void ReturnDoneAll()
        {
            var queue = new Queue<IDomainEvent>();
            queue.Enqueue(new RequisitesEvent("", ""));
            //{"Amount": 212.0000, "ComponentId": 3784495, "Products": [{"Price": 212.0000, "ItemId": "143042310", "Quantity": 1}] }
            queue.Enqueue(new ComponentAddedEvent(3784495, 212, new ProductSourceObject[]{new ProductSourceObject("143042310", 1, 212 ), }));
            //{ "ComponentIds": [3784495], "ProductItemIds": ["143042310"], "Amount": 212.0}
            queue.Enqueue(new V2.ProcessedEvent(new  [] { 3784495L }, new [] { new V2.ItemIdQuantityPair("143042310", 0) }, 212));
            //{"Amount": 2.7500, "ComponentId": 3786228, "Products": []}
            queue.Enqueue(new ComponentAddedEvent(3786228, 2.75M, new ProductSourceObject[0]));

            var aggregate = new Order("111111-111", new EventStream<OrderSnapshotObject>(queue.Count, null, queue));

            aggregate.SetAllProcessed();
        }
    }
}
