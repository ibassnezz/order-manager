namespace OrderManager.Events.EventStack.V1
{

    [EventId("V1.ProcessedEvent")]
    public class ProcessedEvent: IDomainEvent
    {
        public ProcessedEvent(long[] componentIds, string[] productItemIds, decimal amount)
        {
            ComponentIds = componentIds;
            ProductItemIds = productItemIds;
            Amount = amount;
        }
        
        public long[] ComponentIds { get; set; }

        public string[] ProductItemIds { get; set; }

        public decimal Amount { get; set; }
    }
}
