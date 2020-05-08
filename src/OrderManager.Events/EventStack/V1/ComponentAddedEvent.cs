
namespace OrderManager.Events.EventStack.V1
{
    [EventId("V1.ComponentAddedEvent")]
    public class ComponentAddedEvent: IDomainEvent
    {
        public ComponentAddedEvent(long componentId, decimal amount, ProductSourceObject[] products)
        {
            ComponentId = componentId;
            Amount = amount;
            Products = products;
        }

        public ComponentAddedEvent() { }

        public long ComponentId { get; set; }

        public decimal Amount { get; set; }

        public ProductSourceObject[] Products { get; set; }


    }
}
