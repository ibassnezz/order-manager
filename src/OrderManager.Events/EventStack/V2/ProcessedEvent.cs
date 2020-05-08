namespace OrderManager.Events.EventStack.V2
{

    [EventId("V2.ProcessedEvent")]
    public class ProcessedEvent: IDomainEvent
    {
        public ProcessedEvent(long[] componentIds, ItemIdQuantityPair[] productValues, decimal amount)
        {
            ComponentIds = componentIds;
            ProductValues = productValues;
            Amount = amount;
        }

        public ProcessedEvent() { }
        
        public long[] ComponentIds { get; set; }

        public ItemIdQuantityPair[] ProductValues { get; set; }

        public decimal Amount { get; set; }
    }
}
